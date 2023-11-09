using System.Linq;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class HideProgressSliderOnClassificationAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (_contentModel.SelectedAsset.IsClassificationSelected)
            {
                var selectedActivityId = _contentModel.SelectedAsset.Classification.itemId;
                var foundedView = _homeModel.ShownClassificationAssetsOnHome.SingleOrDefault(asset => asset.Id == selectedActivityId);
        
                if (foundedView != null)
                {
                    foundedView.ResetProgress();
                    foundedView.ShowProgressSlider(false);
                }
            }
        }
    }
}