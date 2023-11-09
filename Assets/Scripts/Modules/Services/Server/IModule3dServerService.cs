using System;
using System.Collections.Generic;
using TDL.Models;

namespace TDL.Modules.Model3D
{
    public interface IModule3dServerService
    {
        void GetAssetDetails(int assetId);
        void GetSearch(string searchValue, string cultureCode, string[] assetTypes, Action<List<ClientAssetModel>> clientAssetModels);
    }
}