using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetContentCommand : ICommand
    {
        [Inject] private ServerService _serverService;

        public void Execute()
        {
            _serverService.GetContent();
        }
    }
}