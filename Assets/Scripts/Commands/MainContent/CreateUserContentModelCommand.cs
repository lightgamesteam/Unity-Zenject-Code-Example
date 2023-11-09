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
    public class CreateUserContentModelCommand : ICommandWithParameters
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

        private void RunInBackground(ISignal signal)
        {
            var persistentDataPath = Application.persistentDataPath;

            UnityTask.Run(() =>
                {
                    var parameter = (CreateUserContentModelCommandSignal)signal;

                    var contentResponse =
                        JsonConvert.DeserializeObject<UserContentModel[]>(parameter.UserContentResponse);
                    var userContentList = new List<UserContent>();

                    foreach (var v in contentResponse)
                    {
                        var userContent = new UserContent();
                        userContent.Id = v.Id;
                        userContent.Name = v.Name;
                        userContent.AssetName = v.AssetName;
                        userContent.ContentTypeId = v.ContentTypeId;
                        userContent.ContentFile = v.ContentFile;
                        userContent.ContentFileId = v.ContentFileId;
                        userContent.FileUrl = v.FileUrl;
                        var ContentTypeName = userContent.ContentTypeName;
                        if (userContent.ContentTypeName == null)
                        {
                            ContentTypeName = "";
                        }

                        userContent.ThumbnailUrl = v.ThumbnailUrl;

                        var localFileName = $"{userContent.Id.ToString()}_{userContent.AssetName}_{userContent.Name}";
                        var path = Path.Combine(persistentDataPath, CacheConstants.CacheFolder,
                            CacheConstants.UserContentFolder, ContentTypeName, localFileName);
                        path = v.ContentTypeId == 100 ? path : path + FileExtension.DefaultVideoExtension;
                        userContent.FilePath = path;

                        userContentList.Add(userContent);
                    }

                    return userContentList;
                }
            ).ContinueOnUIThread(task =>
            {
                if (task.Result != null)
                {
                    _userContentAppModel.UpdateUserContent(task.Result);
                    _signal.Fire<ShowMyContentViewedCommandSignal>();
                }
                else
                {
                    _signal.Fire(
                        new SendCrashAnalyticsCommandSignal(
                            "CreateUserContentModelCommand server response: error during creation"));
                }
            });
        }

        private void RunInBackgroundWebGL(ISignal signal)
        {
            var persistentDataPath = Application.persistentDataPath;

            var parameter = (CreateUserContentModelCommandSignal)signal;

            var contentResponse = JsonConvert.DeserializeObject<UserContentModel[]>(parameter.UserContentResponse);
            var userContentList = new List<UserContent>();

            if (contentResponse.Length > 0)
            {
                foreach (var v in contentResponse)
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
                    Debug.Log($"User Content {userContent}");
                    var localFileName = $"{userContent.Id.ToString()}_{userContent.AssetName}_{userContent.Name}";
                    var path = Path.Combine(persistentDataPath, CacheConstants.CacheFolder,
                        CacheConstants.UserContentFolder, userContent.ContentTypeName, localFileName);
                    Debug.Log($"Path is {path}");
                    //Check file extension here
                    var extension = FileExtension.DefaultVideoExtension;
                    var split = userContent.ContentFile.Split('.');
                    if (split.Length > 1 && split.Last() == "webm")
                    {
                        extension = FileExtension.DefaultWebVideoExtension;
                    }
                    
                    Debug.Log($"userContent.ContentFile {userContent.ContentFile}, {extension}");
                    path = v.ContentTypeId == 100 ? path : path + extension;
                    userContent.FilePath = path;
        
                    userContentList.Add(userContent);
                }
            }


            if (userContentList.Count >= 0 /*task.Result != null*/)
            {
                _userContentAppModel.UpdateUserContent(userContentList /*task.Result*/);
                _signal.Fire<ShowMyContentViewedCommandSignal>();
            }
            else
            {
                _signal.Fire(
                    new SendCrashAnalyticsCommandSignal(
                        "CreateUserContentModelCommand server response: error during creation"));
            }
        }
    }
}