using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SaveLastShownActivityCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            _homeModel.LastShownActivity = signal;
        }
    }
}