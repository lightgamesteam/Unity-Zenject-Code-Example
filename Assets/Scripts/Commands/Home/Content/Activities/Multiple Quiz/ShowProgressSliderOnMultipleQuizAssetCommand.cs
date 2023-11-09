using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowProgressSliderOnMultipleQuizAssetCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (IsMultipleActivitySelected())
            {
                var activeId = _contentModel.SelectedAsset.Quiz[0].itemId;
                var quizView = _homeModel.ShownMultipleQuizAssetsOnHome.Find(item => item.Id == activeId);

                if (quizView != null)
                {
                    quizView.ShowProgressSlider(true);
                }
            }
        }

        private bool IsMultipleActivitySelected()
        {
            return _contentModel.SelectedAsset != null 
                   && _contentModel.SelectedAsset.IsMultipleQuizSelected;
        }
    }
}