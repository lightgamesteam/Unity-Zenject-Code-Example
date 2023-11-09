using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class ChangePasswordClickCommand : ICommand
    {
        [Inject] private SignalBus _signal;
        [Inject] private readonly ApplicationSettingsInstaller.ServerSettings serverSettings;
        
        public void Execute()
        {
            _signal.Fire(new OpenUrlCommandSignal(serverSettings.ChangePasswordUrl));
        }
    }
}