using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class PauseAllExceptActiveDescriptionCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (PauseAllExceptActiveDescriptionCommandSignal) signal;

            foreach (var description in _homeModel.OpenedDescriptions)
            {
                var view = description.Value;

                var viewId = string.IsNullOrEmpty(view.LabelId)
                    ? view.AssetId
                    : view.LabelId;

                if (viewId != parameter.Id)
                {
                    view.PlaySound(false);
                }
            }
        }
    }

    public class PauseAllExceptActiveDescriptionCommandSignal : ISignal
    {
        public string Id { get; }

        public PauseAllExceptActiveDescriptionCommandSignal(string id)
        {
            Id = id;
        }
    }
}