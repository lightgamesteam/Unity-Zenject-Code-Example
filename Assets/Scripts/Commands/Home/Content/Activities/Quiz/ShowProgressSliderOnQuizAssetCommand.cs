using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowProgressSliderOnQuizAssetCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartDownloadAssetCommandSignal) signal;
            var assetId = parameter.Id;

            var quizView = _homeModel.ShownQuizAssetsOnHome.Find(item => item.Id == assetId);

            if (quizView != null)
            {
                quizView.ShowProgressSlider(true);
            }
        }
    }
}