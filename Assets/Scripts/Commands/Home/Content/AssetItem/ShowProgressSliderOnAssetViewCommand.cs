using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowProgressSliderOnAssetViewCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartDownloadAssetCommandSignal) signal;
            var assetId = parameter.Id;

            var view = _homeModel.ShownAssetsOnHome.Find(item => item.AssetId == assetId);

            if (view != null)
            {
                view.SetDownloadStatus(true, parameter.UpdateAssetItemDownloadStatus);
                view.ShowProgressSlider(true);
            }
        }
    }
}