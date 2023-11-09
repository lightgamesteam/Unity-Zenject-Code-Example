using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class RemovedFromFavoritesCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (RemovedFromFavouritesCommandSignal) signal;

            var response = JsonConvert.DeserializeObject<ResponseBase>(parameter.Response);

            if (response.Success)
            {
                _contentModel.FavoriteAssets.RemoveAll(asset => asset.IsEqualsIds(parameter.AssetId, parameter.GradeId));
                
                //Force update.
                _signal.Fire(new ShowFavouritesCommandSignal());
            }
            else
            {
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("RemovedFromFavoritesCommand server response | " + response.ErrorMessage));
            }
        }
    }
}