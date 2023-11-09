using CI.TaskParallel;
using Newtonsoft.Json;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class DownloadAvailableResourcesCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            Debug.Log("DownloadAvailableResourcesCommand");
#if UNITY_WEBGL
            RunInBackgroundWebGL(signal);
#else
            RunInBackground(signal);
#endif
        }

        private void RunInBackground(ISignal signal)
        {
            UnityTask.Run(() =>
            {
                var parameter = (DownloadAvailableResourcesCommandSignal) signal;
                var resourceResponse = JsonConvert.DeserializeObject<AppResourceResponse>(parameter.ResourcesResponse);

                if (resourceResponse.Success)
                {
                    if (resourceResponse.Resources.Count > 0)
                    {
                        _serverService.DownloadAvailableResources(resourceResponse.Resources[0].FileUrl);
                    }
                }
                else
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("DownloadAvailableResourcesCommand server response | " + resourceResponse.ErrorMessage));
                }
            });
        }
        
        private void RunInBackgroundWebGL(ISignal signal)
        {
            var parameter = (DownloadAvailableResourcesCommandSignal) signal;
            var resourceResponse = JsonConvert.DeserializeObject<AppResourceResponse>(parameter.ResourcesResponse);

            if (resourceResponse.Success)
            {
                if (resourceResponse.Resources.Count > 0)
                {
                    _serverService.DownloadAvailableResources(resourceResponse.Resources[0].FileUrl);
                }
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("DownloadAvailableResourcesCommand server response | " + resourceResponse.ErrorMessage));
            }
        }
    }
}