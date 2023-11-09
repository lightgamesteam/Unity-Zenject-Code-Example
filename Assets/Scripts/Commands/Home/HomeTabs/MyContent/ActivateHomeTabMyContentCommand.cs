using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ActivateHomeTabMyContentCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.HomeTabMyContentActive = true;
        }
    }
}