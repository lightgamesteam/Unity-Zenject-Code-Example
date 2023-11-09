using TDL.Models;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class LoginSkolonClickCommand: ICommand
    {
        [Inject] 
        private ServerService _serverService;

        [Inject] 
        private PopupOverlayMediator _popupOverlayMediator;
    
        [Inject] 
        private UserLoginModel _userLoginModel;

        [Inject] private SignalBus _signalBus;
    
        [Inject] 
        private LocalizationModel _localizationModel;

        public void Execute()
        {
            _serverService.GetLoginSkolon();
            ShowPopupOverlay();
        }
    
        private void ShowPopupOverlay()
        {
            _signalBus.Fire(new LoginStateSignal(LoginState.Authenticating));

            //_signalBus.Fire(new PopupOverlaySignal(true, _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingKey]));
        
#if  DEBUG_POPUP
        _signalBus.Fire(new PopupOverlaySignal(true, $"Send user credentials \n Time from start: {Math.Round(Time.unscaledTime, 2)} sec"));
#endif
        
        }
    }
}