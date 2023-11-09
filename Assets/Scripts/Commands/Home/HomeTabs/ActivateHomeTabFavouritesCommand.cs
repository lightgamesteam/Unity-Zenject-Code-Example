using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ActivateHomeTabFavouritesCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.HomeTabFavouritesActive = true;
        }
    }
}