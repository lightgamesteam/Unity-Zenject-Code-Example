using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SelectActivityItemCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            _contentModel.SelectedActivity = new ClientActivityModel
            {
                Id = _contentModel.SelectedCategory.Id,
                AllAssets = _contentModel.SelectedCategory.AllAssets,
                AssetsInCategory = _contentModel.SelectedCategory.AssetsInCategory,
                AssetsInChildren = _contentModel.SelectedCategory.AssetsInChildren
            };
        }
    }
}