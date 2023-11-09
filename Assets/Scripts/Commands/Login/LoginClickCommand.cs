using TDL.Models;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class LoginClickCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (LoginClickCommandSignal) signal;

            SendUserInfoOnServer(parameter.UserName, parameter.UserPassword);
            SaveUserInfoToModel(parameter.UserName, parameter.UserPassword, parameter.RememberUser);
            ShowPopupOverlay();
        }

        private void SendUserInfoOnServer(string userName, string userPassword)
        {
            _serverService.GetLogin(userName, userPassword);
        }

        private void SaveUserInfoToModel(string userName, string userPassword, bool rememberUser)
        {
            _userLoginModel.UserLogin = userName;
            _userLoginModel.UserPassword = userPassword;
            _userLoginModel.RememberUser = rememberUser;
        }

        private void ShowPopupOverlay()
        {
            _signal.Fire(new LoginStateSignal(LoginState.Authenticating));
            
#if  DEBUG_POPUP
        _signalBus.Fire(new PopupOverlaySignal(true, $"Send user credentials \n Time from start: {Math.Round(Time.unscaledTime, 2)} sec"));
#endif
        }
    }
}