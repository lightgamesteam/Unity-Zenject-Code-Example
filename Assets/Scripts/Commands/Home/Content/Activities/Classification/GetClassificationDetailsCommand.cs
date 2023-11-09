using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetClassificationDetailsCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (GetClassificationDetailsCommandSignal) signal;
            _serverService.GetClassificationDetails(parameter.Id);
        }
    }
}