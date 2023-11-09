using System.Linq;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CancelMultipleQuizDownloadProgressCommand : ICommand
    {
        [Inject] private SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
    
        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsMultipleQuizSelected)
            {
                var activeId = GetSelectedQuizItemId();
                _homeModel.MultipleAssetIdQueueToDownload.Remove(activeId);

                var multipleQuiz = _contentModel.GetMultipleQuizById(activeId);
                var idsToRemove = multipleQuiz.ActivityItem.assetContent.ToList().Select(asset => asset.assetId);

                foreach (var assetId in idsToRemove)
                {
                    _signal.Fire(new CancelDownloadProgressCommandSignal(assetId));
                }
            }
        }

        private int GetSelectedQuizItemId()
        {
            return _contentModel.SelectedAsset.Quiz[0].itemId;
        }
    }
}