using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetUserContentCommand : ICommand
    {
        [Inject] private ServerService _serverService;

        public void Execute()
        {
            _serverService.GetUserContent();
        }
    }
}


namespace TDL.Signals
{
    public class GetUserContentCommandSignal : ISignal
    {
        
    }
}