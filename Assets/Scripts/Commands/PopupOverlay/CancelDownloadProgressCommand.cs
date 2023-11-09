using System.Linq;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CancelDownloadProgressCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (CancelDownloadProgressCommandSignal) signal;
            var assetId = parameter.Id;

            AbortDownloadRequest(assetId);
            RemoveAssetFromCache(assetId);
        }

        private void AbortDownloadRequest(int assetId)
        {
            var asset = _homeModel.AssetsToDownload.FirstOrDefault(item => item.Id == assetId);
            if (asset != null)
            {
                asset.ItemRequest.Abort();
                _homeModel.AssetsToDownload.Remove(asset);
            }
        }

        private void RemoveAssetFromCache(int assetId)
        {
            var asset = _contentModel.GetAssetById(assetId);
            
            if (_cacheService.IsAssetExists(assetId, asset.Asset.Version))
            {
                _cacheService.DeleteAsset(assetId, asset.Asset.Version);
            }
        }
    }
}