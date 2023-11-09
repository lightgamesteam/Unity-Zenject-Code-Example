using Managers;
using Signals.Login;
using Zenject;

namespace TDL.Commands
{
    public class CheckLoginAsGuestCommand: ICommand
    {
        [Inject] private readonly ExternalCallManager _externalCallManager;
        [Inject] private readonly SignalBus _signal;
        
        public void Execute()
        {
            if (!_externalCallManager.IsTeams)
            {
                _signal.Fire(new LoginAsGuestCommandSignal());
            }
        }
    }
}