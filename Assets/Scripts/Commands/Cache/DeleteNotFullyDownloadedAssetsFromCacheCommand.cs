using System.IO;
using CI.TaskParallel;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class DeleteNotFullyDownloadedAssetsFromCacheCommand : ICommand
    {
        [Inject] private ICacheService _cacheService;

        public void Execute()
        {
            var cacheFolder = _cacheService.GetPathToAssetsFolder();
#if UNITY_WEBGL
            RunInBackgroundWebGL(cacheFolder);
#else
            RunInBackground(cacheFolder);
#endif
        }

        private void RunInBackground(string cacheFolder)
        {
            UnityTask.Run(() =>
            {
                var files = Directory.GetFiles(cacheFolder);

                foreach (var file in files)
                {
                    var fileSize = new FileInfo(file).Length;
                    if (fileSize == 0)
                    {
                        File.Delete(file);
                    }
                }
            });
        }
        
        private void RunInBackgroundWebGL(string cacheFolder)
        {
            var files = Directory.GetFiles(cacheFolder);

            foreach (var file in files)
            {
                var fileSize = new FileInfo(file).Length;
                if (fileSize == 0)
                {
                    File.Delete(file);
                }
            }
        }
    }
}