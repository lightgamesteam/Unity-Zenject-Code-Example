using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SelectClassificationAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartClassificationCommandSignal) signal;

            _contentModel.SelectedAsset = new ClientAssetModel
            {
                Classification = _contentModel.GetClassificationById(parameter.Id).ActivityItem,
                IsClassificationSelected = true
            };
        }
    }
}