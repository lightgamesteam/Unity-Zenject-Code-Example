using TDL.Commands;
using TDL.Models;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class Module3dProcessAssetDetailsCommand : ICommand
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly ContentModel _contentModel;

        public void Execute()
        {
            var signalSource = _contentModel.AssetDetailsSignalSource;

            if (signalSource is DownloadAssetCommandSignal targetSignal)
            {
                _signal.Fire(targetSignal);
            }
        }
    }
}