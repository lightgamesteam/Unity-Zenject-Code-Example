using System.Collections.Generic;
using TDL.Core;
using TDL.Server;

namespace TDL.Models
{
    public class ClientActivityModel : ICategory
    {
        public int Id { get; set; }
        public List<ClientAssetModel> AssetsInCategory { get; set; }
        public List<ClientAssetModel> AssetsInChildren { get; set; }
        public List<ClientAssetModel> AllAssets { get; set; }
        public ActivityItem ActivityItem { get; set; }
        public List<Dictionary<ContentModel.CategoryNames, int>> DirectPathToActivity { get; set; } = new List<Dictionary<ContentModel.CategoryNames, int>>();
    }
}