using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class DownloadAssetDetailsCommand : ICommandWithParameters
    {
        [Inject] private ServerService _server;

        public void Execute(ISignal signal)
        {
            var parameter = (DownloadAssetDetailsCommandSignal) signal;

            _server.GetAssetDetails(parameter.Id, parameter.GradeId);
        }
    }
}