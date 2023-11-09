using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ActivateHomeTabMyTeacherCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.HomeTabMyTeacherActive = true;
        }
    }
}