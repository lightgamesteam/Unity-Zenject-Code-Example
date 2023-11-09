using System.Collections.Generic;
using TDL.Core;
using TDL.Server;

namespace TDL.Models
{
    public class ClientSubtopicModel : ICategory
    {
        public int Id { get; set; }
        public ClientTopicModel ParentTopic { get; set; }
        public Subtopic Subtopic { get; set; }
        public List<ClientAssetModel> AssetsInCategory { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AssetsInChildren { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AllAssets { get; set; } = new List<ClientAssetModel>();
    }
}