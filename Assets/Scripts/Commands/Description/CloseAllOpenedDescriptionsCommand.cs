using System.Linq;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class CloseAllOpenedDescriptionsCommand : ICommand
    {
        [Inject] private readonly HomeModel _homeModel;

        public void Execute()
        {
            for (var index = _homeModel.OpenedDescriptions.Count - 1; index >= 0; index--)
            {
                _homeModel.OpenedDescriptions.ElementAt(index).Value.OnCloseClick();
            }
        }
    }

    public class CloseAllOpenedDescriptionsCommandSignal : ISignal
    {
    }
}