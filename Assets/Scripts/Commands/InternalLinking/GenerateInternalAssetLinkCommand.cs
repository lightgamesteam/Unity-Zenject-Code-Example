using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class GenerateInternalAssetLinkCommand: ICommandWithParameters
    {
        [Inject] private ServerService _serverService;
        
        public void Execute(ISignal signal)
        {
            var parameter = (GenerateInternalAssetLinkCommandSignal)signal;
            _serverService.GetGenerateInternalAssetLink(parameter.AssetId);
        }
    }
}

namespace TDL.Signals
{
    public class GenerateInternalAssetLinkCommandSignal : ISignal
    {
        public string AssetId { get; private set; }

        public GenerateInternalAssetLinkCommandSignal(string assetId)
        {
            AssetId = assetId;
        }
    }
}