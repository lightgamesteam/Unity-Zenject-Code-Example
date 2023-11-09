using Signals.Tools;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetAvailableResourcesCommand : ICommand
    {
        [Inject] private ServerService _server;
        [Inject] private SignalBus _signal;
    
        public void Execute()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            _signal.Fire<RequestWebGLMicrophoneSignal>();
#endif
            _server.GetAvailableResources();
        }
    }
}