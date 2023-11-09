using System.Linq;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class UpdateAssetDownloadProgressCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (UpdateDownloadProgressCommandSignal) signal;

            var progress = parameter.Downloaded / (float) parameter.Length * 100.0f;
            var foundItem = _homeModel.AssetsToDownload.FirstOrDefault(item => item.Id == parameter.ItemId);

            if (foundItem != null)
            {
                var index = _homeModel.AssetsToDownload.IndexOf(foundItem);
                foundItem.Progress = progress;
                _homeModel.AssetsToDownload[index] = foundItem;
            }
        }
    }
}