using Signals.Login;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class LoadAssetsAsGuestLoginScreenCommand : ICommand
    {
        [Inject] private SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private ServerService _server;
        
        public void Execute()
        {
            _contentModel.OnContentModelChangedAsGuest = OnLoaded;
            _server.GetContent();
        }

        private void OnLoaded()
        {
            _signal.Fire(new MainScreenCreateAssetsCommandSignal()); //OSA Scroll
        }
    }
}