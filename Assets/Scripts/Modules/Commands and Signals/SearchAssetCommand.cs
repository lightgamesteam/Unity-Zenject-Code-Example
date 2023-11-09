using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TDL.Models;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class SearchAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
    
        public void Execute(ISignal signal)
        {
            var parameter = (SearchAssetCommandSignal) signal;
            var searchResponse = JsonConvert.DeserializeObject<int[]>(parameter.Response);
            parameter.ClientAssetModels.Invoke(_contentModel.GetSearchAssetsByType(searchResponse, parameter.AssetTypes));
        }
    }

    public class SearchAssetCommandSignal : ISignal
    {
        public string Response;
        public string[] AssetTypes;
        public Action<List<ClientAssetModel>> ClientAssetModels;
    
        public SearchAssetCommandSignal(string response, string[] assetTypes, Action<List<ClientAssetModel>> clientAssetModels)
        {
            Response = response;
            AssetTypes = assetTypes;
            ClientAssetModels = clientAssetModels;
        }
    }
}