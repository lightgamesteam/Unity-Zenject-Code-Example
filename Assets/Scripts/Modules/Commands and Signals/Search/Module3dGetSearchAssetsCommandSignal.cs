using System;
using System.Collections.Generic;
using TDL.Models;

namespace TDL.Modules.Model3D
{
    public class Module3dGetSearchAssetsCommandSignal : ISignal
    {
        public string SearchValue;
        public string CultureCode;
        public string[] AssetTypes;
        public Action<List<ClientAssetModel>> ClientAssetModels;
    
        public Module3dGetSearchAssetsCommandSignal(string searchValue, string cultureCode, string[] assetTypes, Action<List<ClientAssetModel>> clientAssetModels)
        {
            SearchValue = searchValue;
            CultureCode = cultureCode;
            AssetTypes = assetTypes;
            ClientAssetModels = clientAssetModels;
        }
    }
}