using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class HideProgressSliderOnQuizAssetCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (CancelDownloadProgressCommandSignal) signal;
            var assetId = parameter.Id;
        
            var clickedView = _homeModel.ShownQuizAssetsOnHome.Find(item => item.Id == assetId);

            if (clickedView != null)
            {
                clickedView.ResetProgress();
            }
        }
    }
}