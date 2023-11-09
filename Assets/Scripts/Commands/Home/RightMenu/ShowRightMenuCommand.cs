using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class ShowRightMenuCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (ShowRightMenuCommandSignal) signal;
            _homeModel.IsRightMenuActive = parameter.IsEnabled;
        }
    }
}