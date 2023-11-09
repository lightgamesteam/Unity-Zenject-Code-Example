using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ClearLastShownActivityCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.LastShownActivity = null;
        }
    }
}