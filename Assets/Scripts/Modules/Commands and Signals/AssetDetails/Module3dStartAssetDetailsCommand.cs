using TDL.Models;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class Module3dStartAssetDetailsCommand : ICommandWithParameters
    {
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (Module3dStartAssetDetailsCommandSignal) signal;

            var asset = _contentModel.GetAssetById(parameter.Id);
            if (asset.AssetDetail == null)
            {
                _signal.Fire(new Module3dDownloadAssetDetailsCommandSignal(parameter.Id));
            }
            else
            {
                _signal.Fire(new Module3dProcessAssetDetailsCommandSignal(parameter.Id));
            }
        }
    }
}