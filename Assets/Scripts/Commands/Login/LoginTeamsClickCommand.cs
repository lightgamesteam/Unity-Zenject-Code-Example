using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class LoginTeamsClickCommand: ICommand
    {
        [Inject] 
        private ServerService _serverService;

        [Inject] 
        private PopupOverlayMediator _popupOverlayMediator;

        [Inject] private SignalBus _signalBus;

        public void Execute()
        {
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