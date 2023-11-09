using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SelectActivityQuizAssetsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            _contentModel.SelectedChosenActivity.IsSelected = true;
            _contentModel.SelectedChosenActivity.ActivityName = LocalizationConstants.ActivityQuizzesKey;
            _contentModel.SelectedChosenActivity.AllAssets = _contentModel.SelectedCategory.AllAssets;
        
            _contentModel.ForceUpdateChosenActivity();
        }
    }
}