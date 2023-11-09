using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowLeftMenuCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (ShowLeftMenuCommandSignal) signal;
            _homeModel.IsLeftMenuActive = parameter.IsEnabled;
        }
    }
}