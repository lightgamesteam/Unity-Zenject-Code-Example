using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class HideProgressSliderOnPuzzleAssetCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (CancelDownloadProgressCommandSignal) signal;
            var assetId = parameter.Id;
        
            var clickedView = _homeModel.ShownPuzzleAssetsOnHome.Find(item => item.AssetId == assetId);

            if (clickedView != null)
            {
                clickedView.ResetProgress();
            }
        }
    }
}