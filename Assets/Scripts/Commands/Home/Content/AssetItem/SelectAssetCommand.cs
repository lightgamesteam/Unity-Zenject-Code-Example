using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SelectAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (AssetItemClickCommandSignal) signal;

            var asset = _contentModel.GetAssetById(parameter.Id);

            if (parameter.GradeId > 0)
            {
                _contentModel.SelectedGrade = _contentModel.GetGradeById(parameter.GradeId);
                asset.Asset.GradeId = parameter.GradeId;
            }

            _contentModel.SelectedAsset = asset;
        }
    }
}