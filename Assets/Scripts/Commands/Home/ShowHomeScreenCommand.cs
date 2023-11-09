using TDL.Constants;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class ShowHomeScreenCommand : ICommand
    {
        [Inject] private IWindowService _windowService;
        [Inject] private PopupOverlayMediator _popupOverlayMediator;
        [Inject] private SignalBus _signalBus;

        public void Execute()
        {
            _windowService.ShowWindow(WindowConstants.Home);
            _signalBus.Fire(new PopupOverlaySignal(false));
        }
    }
}