using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowProgressSliderOnMultiplePuzzleAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (IsMultipleActivitySelected())
            {
                var activeId = _contentModel.SelectedAsset.Puzzle[0].itemId;
                var puzzleView = _homeModel.ShownMultiplePuzzleAssetsOnHome.Find(item => item.Id == activeId);

                if (puzzleView != null)
                {
                    puzzleView.ShowProgressSlider(true);
                }
            }
        }

        private bool IsMultipleActivitySelected()
        {
            return _contentModel.SelectedAsset != null 
                   && _contentModel.SelectedAsset.IsMultiplePuzzleSelected;
        }
    }
}