using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ActivateHomeTabContentCommand : ICommand
    {
        [Inject] private readonly UserContentAppModel _userContentAppModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly HomeModel _homeModel;

        public void Execute()
        {
            if (_userContentAppModel.IsTeacherContent)
            {
                _homeModel.HomeTabMyTeacherActive = true;
            }
            else
            {
                _homeModel.HomeTabMyContentActive = true;
            }
        }
    }   
}
