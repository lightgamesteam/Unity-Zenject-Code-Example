using System.Linq;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CancelMultiplePuzzleDownloadProgressCommand : ICommand
    {
        [Inject] private SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
    
        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsMultiplePuzzleSelected)
            {
                var activeId = GetSelectedPuzzleItemId();
                _homeModel.MultipleAssetIdQueueToDownload.Remove(activeId);

                var multipleQuiz = _contentModel.GetMultiplePuzzleById(activeId);
                var idsToRemove = multipleQuiz.ActivityItem.assetContent.ToList().Select(asset => asset.assetId);

                foreach (var assetId in idsToRemove)
                {
                    _signal.Fire(new CancelDownloadProgressCommandSignal(assetId));
                }
            }
        }
        
        private int GetSelectedPuzzleItemId()
        {
            return _contentModel.SelectedAsset.Puzzle[0].itemId;
        }
    }
}