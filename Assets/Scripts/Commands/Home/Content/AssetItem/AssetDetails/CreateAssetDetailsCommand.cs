using System.Collections.Generic;
using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CreateAssetDetailsCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (CreateAssetDetailsCommandSignal) signal;

            var assetDetails = JsonConvert.DeserializeObject<List<AssetDetail>>(parameter.Response)[0];

            var asset = _contentModel.GetAssetById(parameter.Id);
            asset.AssetDetail = assetDetails;
            asset.AssetDetail.AssetContentPlatform = assetDetails.AssetContents[0];

            if (_contentModel.MultipleAssetDetailsIds.Count > 0)
            {
                _contentModel.MultipleAssetDetailsIds.Remove(parameter.Id);
            }

            if (_contentModel.MultipleAssetDetailsIds.Count == 0)
            {
                _signal.Fire(new ProcessAssetDetailsCommandSignal(parameter.Id));
            }
        }
    }
}