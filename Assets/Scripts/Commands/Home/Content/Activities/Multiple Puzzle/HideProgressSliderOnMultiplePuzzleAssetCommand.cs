using System.Linq;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class HideProgressSliderOnMultiplePuzzleAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsMultiplePuzzleSelected)
            {
                var selectedActivityId = GetSelectedPuzzleItemId();
                var foundedView = _homeModel.ShownMultiplePuzzleAssetsOnHome.SingleOrDefault(asset => asset.Id == selectedActivityId);
        
                if (foundedView != null)
                {
                    foundedView.ResetProgress();
                    foundedView.ShowProgressSlider(false);
                }
            }
        }
        
        private int GetSelectedPuzzleItemId()
        {
            return _contentModel.SelectedAsset.Puzzle[0].itemId;
        }
    }
}