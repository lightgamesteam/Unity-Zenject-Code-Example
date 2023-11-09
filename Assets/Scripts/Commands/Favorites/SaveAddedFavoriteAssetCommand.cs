using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveAddedFavoriteAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveAddedFavouriteAssetCommandSignal) signal;

            var response = JsonConvert.DeserializeObject<ResponseBase>(parameter.Response);

            if (response.Success)
            {
                var clientAsset = _contentModel.GetAssetById(parameter.AssetId);
                
                var favAsset = clientAsset.Clone();
                favAsset.Asset.GradeId = parameter.GradeId;
                
                _contentModel.FavoriteAssets.Add(favAsset);
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("SaveAddedFavoriteAssetCommand server response | " + response.ErrorMessage));
            }
        }
    }
}