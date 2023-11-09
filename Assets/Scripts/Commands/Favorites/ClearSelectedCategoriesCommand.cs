using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ClearSelectedCategoriesCommand : ICommand
    {
        [Inject] private readonly ContentModel _contentModel;

        public void Execute()
        {
            _contentModel.ClearSelectedCategories();
        }
    }
}