using System.IO;
using System.Linq;
using CI.TaskParallel;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SaveAssetInCacheCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ICacheService _cacheService;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveAssetInCacheCommandSignal) signal;

            var foundItem = _homeModel.AssetsToDownload.FirstOrDefault(item => item.Id == parameter.Id);

            if (foundItem != null)
            {
                var pathToAsset = _cacheService.GetPathToAsset(foundItem.Id, foundItem.Version);
#if UNITY_WEBGL
                RunInBackgroundWebGL(pathToAsset, parameter, foundItem);
#else
                RunInBackground(pathToAsset, parameter, foundItem);
#endif

            }
        }

        private void RunInBackground(string pathToAsset, SaveAssetInCacheCommandSignal parameter, HomeModel.DownloadAsset foundItem)
        {
            UnityTask.Run(() =>
            {
                using (var fs = new FileStream(pathToAsset, FileMode.OpenOrCreate))
                {
                    fs.Write(parameter.DataResponse, 0, parameter.DataResponse.Length);
                }
                    
            }
            ).ContinueOnUIThread(task =>
            {
                _homeModel.AssetsToDownload.Remove(foundItem);
            });
        }
        
        private void RunInBackgroundWebGL(string pathToAsset, SaveAssetInCacheCommandSignal parameter, HomeModel.DownloadAsset foundItem)
        {
            using (var fs = new FileStream(pathToAsset, FileMode.OpenOrCreate))
            {
                fs.Write(parameter.DataResponse, 0, parameter.DataResponse.Length);
            }
            
            _homeModel.AssetsToDownload.Remove(foundItem);
        }
    }
}