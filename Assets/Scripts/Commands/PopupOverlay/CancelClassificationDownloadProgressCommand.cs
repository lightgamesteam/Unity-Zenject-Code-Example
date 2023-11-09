using System.Linq;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CancelClassificationDownloadProgressCommand : ICommand
    {
        [Inject] private SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsClassificationSelected)
            {
                var activeId = _contentModel.SelectedAsset.Classification.itemId;
                _homeModel.MultipleAssetIdQueueToDownload.Remove(activeId);

                var classification = _contentModel.GetClassificationById(_contentModel.SelectedAsset.Classification.itemId);
                var idsToRemove = classification.ActivityItem.assetContent.ToList().Select(asset => asset.assetId);

                foreach (var assetId in idsToRemove)
                {
                    _signal.Fire(new CancelDownloadProgressCommandSignal(assetId));
                }
            }
        }
    }
}