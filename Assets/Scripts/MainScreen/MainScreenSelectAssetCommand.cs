using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class MainScreenSelectAssetCommand : ICommandWithParameters
    {
        [Inject] private MainScreenModel _mainModel;
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            //var parameter = (MainScreenAssetItemClickCommandSignal) signal;
            //var asset = _mainModel.GetAssetById(parameter.Id);

            //_contentModel.SelectedAsset = asset;
        }
    }
}