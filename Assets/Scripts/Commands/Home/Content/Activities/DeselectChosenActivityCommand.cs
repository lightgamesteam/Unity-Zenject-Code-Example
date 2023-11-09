using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class DeselectChosenActivityCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;

        public void Execute()
        {
            if (_contentModel.SelectedChosenActivity.IsSelected)
            {
                _contentModel.SelectedChosenActivity.IsSelected = false;
            }
        }
    }
}