using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowProgressSliderOnPuzzleAssetCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartDownloadAssetCommandSignal) signal;
            var assetId = parameter.Id;

            var quizView = _homeModel.ShownPuzzleAssetsOnHome.Find(item => item.AssetId == assetId);

            if (quizView != null)
            {
                quizView.ShowProgressSlider(true);
            }
        }
    }
}