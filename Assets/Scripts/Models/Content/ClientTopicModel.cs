using System.Collections.Generic;
using TDL.Core;
using TDL.Server;

namespace TDL.Models
{
    public class ClientTopicModel : ICategory
    {
        public int Id { get; set; }
        public ClientSubjectModel ParentSubject { get; set; }
        public Topic Topic { get; set; }
        public List<ClientSubtopicModel> ClientSubtopics { get; set; } = new List<ClientSubtopicModel>();
        public List<ClientAssetModel> AssetsInCategory { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AssetsInChildren { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AllAssets { get; set; } = new List<ClientAssetModel>();
    }
}