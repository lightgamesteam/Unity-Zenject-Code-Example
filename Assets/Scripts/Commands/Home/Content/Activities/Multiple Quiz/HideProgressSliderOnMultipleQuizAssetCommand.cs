using System.Linq;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class HideProgressSliderOnMultipleQuizAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsMultipleQuizSelected)
            {
                var selectedActivityId = GetSelectedQuizItemId();
                var foundedView = _homeModel.ShownMultipleQuizAssetsOnHome.SingleOrDefault(asset => asset.Id == selectedActivityId);
        
                if (foundedView != null)
                {
                    foundedView.ResetProgress();
                    foundedView.ShowProgressSlider(false);
                }
            }
        }
        
        private int GetSelectedQuizItemId()
        {
            return _contentModel.SelectedAsset.Quiz[0].itemId;
        }
    }
}