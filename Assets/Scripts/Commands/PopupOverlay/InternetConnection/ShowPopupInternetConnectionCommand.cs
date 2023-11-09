using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowPopupInternetConnectionCommand : ICommand
    {
        [Inject] private readonly PopupModel _popupModel;

        public void Execute()
        {
            _popupModel.ShowInternetConnection = true;
        }
    }
}