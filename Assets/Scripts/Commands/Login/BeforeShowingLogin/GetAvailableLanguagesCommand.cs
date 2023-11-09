using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetAvailableLanguagesCommand : ICommand
    {
        [Inject] private ServerService _server;
    
        public void Execute()
        {
            _server.GetAvailableLanguages();               
        }
    }
}