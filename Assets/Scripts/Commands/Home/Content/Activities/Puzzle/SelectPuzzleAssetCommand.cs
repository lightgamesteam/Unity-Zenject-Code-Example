using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SelectPuzzleAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (StartPuzzleCommandSignal) signal;

            _contentModel.SelectedAsset = _contentModel.GetAssetById(parameter.Id);
            _contentModel.SelectedAsset.IsPuzzleSelected = true;
            _contentModel.SelectedAsset.SelectedPuzzleItemId = parameter.SelectedPuzzleItemId;
        }
    }
}