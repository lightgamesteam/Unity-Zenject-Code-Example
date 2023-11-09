using System.Collections.Generic;
using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using Zenject;

namespace TDL.Modules.Model3D
{
        public class Module3dCreateAssetDetailsCommand : ICommandWithParameters
        {
            [Inject] private readonly ContentModel _contentModel;
            [Inject] private readonly SignalBus _signal;
    
            public void Execute(ISignal signal)
            {
                var parameter = (Module3dCreateAssetDetailsCommandSignal) signal;
    
                var assetDetails = JsonConvert.DeserializeObject<List<AssetDetail>>(parameter.Response)[0];
    
                var asset = _contentModel.GetAssetById(parameter.Id);
                asset.AssetDetail = assetDetails;
                asset.AssetDetail.AssetContentPlatform = assetDetails.AssetContents[0];

                _signal.Fire(new Module3dProcessAssetDetailsCommandSignal(parameter.Id));
            }
        }
}