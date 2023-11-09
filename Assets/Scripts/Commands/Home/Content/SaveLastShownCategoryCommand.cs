using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SaveLastShownCategoryCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            _homeModel.LastShownCategory = signal;
        }
    }
}