using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowCategoryWithHeaderCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
    
        public void Execute()
        {
            _contentModel.IsCategoryWithHeadersActive = true;
        }
    }
}