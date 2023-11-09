using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class GetSearchAssetsCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (GetSearchAssetsCommandSignal) signal;

            _serverService.GetSearch(parameter.SearchValue, parameter.CultureCode);
        }
    }
}