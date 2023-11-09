using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class RemoveDescriptionFromArrayCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (RemoveDescriptionFromArrayCommandSignal) signal;

            var id = !string.IsNullOrEmpty(parameter.LabelId)
                ? parameter.LabelId
                : parameter.AssetId;
            
            if (_homeModel.OpenedDescriptions.ContainsKey(id))
            {
                _homeModel.OpenedDescriptions.Remove(id);
            }   
        }
    }

    public class RemoveDescriptionFromArrayCommandSignal : ISignal
    {
        public string AssetId { get; }
        public string LabelId { get; }

        public RemoveDescriptionFromArrayCommandSignal(string assetId, string labelId)
        {
            AssetId = assetId;
            LabelId = labelId;
        }
    }
}