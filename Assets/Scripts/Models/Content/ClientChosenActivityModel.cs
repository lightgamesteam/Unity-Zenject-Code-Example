using System.Collections.Generic;
using TDL.Core;

namespace TDL.Models
{
    public class ClientChosenActivityModel : ICategory
    {
        public bool IsSelected { get; set; }
        public string ActivityName { get; set; }
        public int Id { get; set; }
        public List<ClientAssetModel> AssetsInCategory { get; set; }
        public List<ClientAssetModel> AssetsInChildren { get; set; }
        public List<ClientAssetModel> AllAssets { get; set; }
    }
}