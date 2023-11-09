using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class CancelAllDownloadsCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            foreach (var downloadingAsset in _homeModel.AssetsToDownload)
            {
                downloadingAsset.ItemRequest.Abort();
            }
            
            _homeModel.AssetsToDownload.Clear();
        }
    }
}