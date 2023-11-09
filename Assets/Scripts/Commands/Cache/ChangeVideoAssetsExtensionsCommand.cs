using System.IO;
using CI.TaskParallel;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class ChangeVideoAssetsExtensionsCommand : ICommand
    {
        [Inject] private ICacheService _cacheService;

        public void Execute()
        {
            var assetsFolder = _cacheService.GetPathToAssetsFolder();
#if UNITY_WEBGL
            RunInBackgroundWebGL(assetsFolder);
#else
            RunInBackground(assetsFolder);
#endif
        }
    
        private void RunInBackground(string assetsFolder)
        {
            UnityTask.Run(() =>
            {
                var assets = Directory.GetFiles(assetsFolder);

                foreach (var asset in assets)
                {
                    var assetExtension = Path.GetExtension(asset);
                    if (assetExtension.Equals(_cacheService.VideoAssetExtension()))
                    {
                        var assetWithChangedExtension = _cacheService.GetEncryptedVideoAssetPath(asset);
                        if (File.Exists(assetWithChangedExtension))
                        {
                            File.Delete(asset);
                        }
                        else
                        {
                            File.Move(asset, assetWithChangedExtension);
                        }
                    }
                }
            });
        }

        private void RunInBackgroundWebGL(string assetsFolder)
        {
            var assets = Directory.GetFiles(assetsFolder);

            foreach (var asset in assets)
            {
                var assetExtension = Path.GetExtension(asset);
                if (assetExtension.Equals(_cacheService.VideoAssetExtension()))
                {
                    var assetWithChangedExtension = _cacheService.GetEncryptedVideoAssetPath(asset);
                    if (File.Exists(assetWithChangedExtension))
                    {
                        File.Delete(asset);
                    }
                    else
                    {
                        File.Move(asset, assetWithChangedExtension);
                    }
                }
            }
        }
    }
}