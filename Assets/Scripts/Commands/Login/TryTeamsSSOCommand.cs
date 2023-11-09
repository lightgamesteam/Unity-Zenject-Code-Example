using Managers;
using Module;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class TryTeamsSSOCommand: ICommand
    {
        [Inject] private readonly ExternalCallManager _externalCallManager;
        [Inject] private readonly ServerService _serverService;
        [Inject] private SignalBus _signal;
        public void Execute()
        {
            if (_externalCallManager.IsTeams)
            {
                _signal.Fire(new LoginStateSignal(LoginState.Authenticating));
                if (_externalCallManager.IsSSOSet)
                {
                    Debug.Log("SSO is set, so calling API constantly");
                    _serverService.GetTeamsSSO(_externalCallManager.SSOAuthToken);
                }
                else
                {
                    Debug.Log("SSO is not set yet, so calling API on action");
                    _externalCallManager.OnSSOSet += delegate
                    {
                        _serverService.GetTeamsSSO(_externalCallManager.SSOAuthToken);
                    };
                }
            }
        }
    }
}