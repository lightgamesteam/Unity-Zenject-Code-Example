using System.Collections.Generic;
using TDL.Models;

namespace TDL.Core
{
    public interface ICategory
    {
        int Id { get; set; }
        List<ClientAssetModel> AssetsInCategory { get; set; }
        List<ClientAssetModel> AssetsInChildren { get; set; }
        List<ClientAssetModel> AllAssets { get; set; }
    }
}