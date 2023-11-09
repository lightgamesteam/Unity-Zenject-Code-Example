using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class RightMenuUnselectAllItemsCommand : ICommand
    {
        [Inject] private readonly HomeModel _homeModel;

        public void Execute()
        {
            if (_homeModel.SelectedMenuItem != null)
            {
                _homeModel.SelectedMenuItem.Deselect();
                _homeModel.SelectedMenuItem = null;
            }

            _homeModel.HomeTabFavouritesActive = false;
            _homeModel.HomeTabRecentActive = false;
            _homeModel.HomeTabMyContentActive = false;
            _homeModel.HomeTabMyTeacherActive = false;
        }
    }
} 