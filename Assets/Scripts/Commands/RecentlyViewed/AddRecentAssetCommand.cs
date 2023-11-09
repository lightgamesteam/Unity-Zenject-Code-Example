using TDL.Models;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class AddRecentAssetCommand : ICommand
    {
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly ServerService _serverService;

        public void Execute()
        {
            var selectedGradeId = _contentModel.SelectedAsset.Asset.GradeId;
            var selectedAssetId = _contentModel.SelectedAsset.Asset.Id;
            _serverService.AddRecentAsset(selectedGradeId, selectedAssetId);
        }
    }
}