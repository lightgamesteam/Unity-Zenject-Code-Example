using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SelectActivityMultipleQuizAssetsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            _contentModel.SelectedChosenActivity.IsSelected = true;
            _contentModel.SelectedChosenActivity.ActivityName = LocalizationConstants.ActivityMultipleQuizzesKey;
            _contentModel.SelectedChosenActivity.AllAssets = _contentModel.SelectedCategory.AllAssets;
        
            _contentModel.ForceUpdateChosenActivity();
        }
    }
}