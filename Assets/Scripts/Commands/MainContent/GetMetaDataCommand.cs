using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetMetaDataCommand : ICommand
    {
        [Inject] private ServerService _serverService;

        public void Execute()
        {
            _serverService.GetMetaData();
        }
    }
}
