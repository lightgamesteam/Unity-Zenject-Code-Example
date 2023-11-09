using System.Collections.Generic;
using System.IO;
using System.Linq;
using CI.TaskParallel;
using Newtonsoft.Json;
using TDL.Constants;
using Zenject;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class CreateTeacherContentModelCommand : ICommandWithParameters
    {
        [Inject] private readonly UserContentAppModel _userContentAppModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ICacheService _cacheService;
        
        public void Execute(ISignal signal)
        {
#if UNITY_WEBGL
            RunInBackgroundWebGL(signal);
#else
            RunInBackground(signal);

#endif
        }

        private void RunInBackgroundWebGL(ISignal signal)
        {
            var errorMessage = string.Empty;
            ResponseBase errorResponse = null;
            var persistentDataPath = Application.persistentDataPath;
            var parameter = (CreateTeacherContentModelCommandSignal) signal;

            var contentResponse = JsonConvert.DeserializeObject<TeacherResponse>(parameter.ContentResponse);
            var userContentList = new List<UserContent>();

            if (contentResponse.Success && contentResponse.UserAssignedContent != null)
            {
                foreach (var v in contentResponse.UserAssignedContent)
                {
                    var userContent = new UserContent();
                    userContent.Id = v.Id;
                    userContent.Name = v.Name;
                    userContent.AssetName = v.AssetName;
                    userContent.ContentTypeId = v.ContentTypeId;
                    userContent.ContentFile = v.ContentFile;
                    userContent.ContentFileId = v.ContentFileId;
                    userContent.FileUrl = v.FileUrl;
                    userContent.ContentTypeName = UserContentTypeNameConstants.GetName(v.ContentTypeId);
                    userContent.ThumbnailUrl = v.ThumbnailUrl;
                    
                    var localFileName = $"{userContent.Id.ToString()}_{userContent.AssetName}_{userContent.Name}";
                    var path = Path.Combine(persistentDataPath, CacheConstants.CacheFolder, CacheConstants.TeacherContentFolder, userContent.ContentTypeName, localFileName);
                    //Check file extension here
                    var extension = FileExtension.DefaultVideoExtension;
                    var split = userContent.ContentFile.Split('.');
                    if (split.Length > 1 && split.Last() == "webm")
                    {
                        extension = FileExtension.DefaultWebVideoExtension;
                    }
                    path = v.ContentTypeId == 100 ? path : path + extension;
                    userContent.FilePath = path;
                    
                    userContentList.Add(userContent);
                }
            }

            if (userContentList.Count >= 0)
            {
                _userContentAppModel.UpdateUserContent(userContentList);
                _signal.Fire<ShowMyContentViewedCommandSignal>();
            }
            else
            {
                _signal.Fire(new SendCrashAnalyticsCommandSignal("Create Teacher Content error: " + errorMessage));
            }
                
        }

        private void RunInBackground(ISignal signal)
        {
            var errorMessage = string.Empty;
            ResponseBase errorResponse = null;
            var persistentDataPath = Application.persistentDataPath;

            UnityTask.Run(() =>
            {
                var parameter = (CreateTeacherContentModelCommandSignal) signal;

                var contentResponse = JsonConvert.DeserializeObject<TeacherResponse>(parameter.ContentResponse);
                var userContentList = new List<UserContent>();

                if (contentResponse.Success && contentResponse.UserAssignedContent != null)
                {
                    foreach (var v in contentResponse.UserAssignedContent)
                    {
                        var userContent = new UserContent();
                        userContent.Id = v.Id;
                        userContent.Name = v.Name;
                        userContent.AssetName = v.AssetName;
                        userContent.ContentTypeId = v.ContentTypeId;
                        userContent.ContentFile = v.ContentFile;
                        userContent.ContentFileId = v.ContentFileId;
                        userContent.FileUrl = v.FileUrl;
                        userContent.ContentTypeName = UserContentTypeNameConstants.GetName(v.ContentTypeId);
                        userContent.ThumbnailUrl = v.ThumbnailUrl;
                    
                        var localFileName = $"{userContent.Id.ToString()}_{userContent.AssetName}_{userContent.Name}";
                        var path = Path.Combine(persistentDataPath, CacheConstants.CacheFolder, CacheConstants.TeacherContentFolder, userContent.ContentTypeName, localFileName);
                        path = v.ContentTypeId == 100 ? path : path + FileExtension.DefaultVideoExtension;
                        userContent.FilePath = path;
                    
                        userContentList.Add(userContent);
                    }
                }
                
                return userContentList; 

            }).ContinueOnUIThread(task =>
            {
                if (task.Result != null)
                {
                    _userContentAppModel.UpdateUserContent(task.Result);
                    _signal.Fire<ShowMyContentViewedCommandSignal>();
                }
                else
                {
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("Create Teacher Content error: " + errorMessage));
                }
            });
        }
    }
}