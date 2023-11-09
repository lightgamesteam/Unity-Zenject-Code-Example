using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowProgressSliderOnClassificationAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (IsMultipleActivitySelected())
            {
                var activeId = _contentModel.SelectedAsset.Classification.itemId;
                var classification = _homeModel.ShownClassificationAssetsOnHome.Find(item => item.Id == activeId);

                if (classification != null)
                {
                    classification.ShowProgressSlider(true);
                }
            }
        }

        private bool IsMultipleActivitySelected()
        {
            return _contentModel.SelectedAsset != null && _contentModel.SelectedAsset.IsClassificationSelected;
        }
    }
}