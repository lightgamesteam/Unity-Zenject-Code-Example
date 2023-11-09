using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class DeactivateAllHomeTabsCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.HomeTabFavouritesActive = false;
            _homeModel.HomeTabRecentActive = false;
            _homeModel.HomeTabMyContentActive = false;
            _homeModel.HomeTabMyTeacherActive = false;
        }
    }
}