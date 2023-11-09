using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ActivateHomeTabRecentCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.HomeTabRecentActive = true;
        }
    }
}