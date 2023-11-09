using System.Linq;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class CloseAllOpenedDescriptionsOnSecondMultiViewCommand : ICommand
    {
        [Inject] private readonly HomeModel _homeModel;

        public void Execute()
        {
            var studentToClose = _homeModel.OpenedDescriptions.Where(label => label.Value.IsMultiViewSecond).ToList();
            
            for (var index = studentToClose.Count - 1; index >= 0; index--)
            {
                studentToClose[index].Value.OnCloseClick();
            }
        }
    }

    public class CloseAllOpenedDescriptionsOnSecondMultiViewCommandSignal : ISignal
    {
    }
}