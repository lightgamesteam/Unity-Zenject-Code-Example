using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ResetPopupOverlayCommand : ICommand
    {
        [Inject] private PopupModel _popupModel;

        public void Execute()
        {
            _popupModel.Reset();
        }
    }
}