using System.Collections.Generic;
using TDL.Core;
using TDL.Server;

namespace TDL.Models
{
    public class ClientSubjectModel : ICategory
    {
        public int Id { get; set; }
        public ClientGradeModel ParentGrade { get; set; }
        public Subject Subject { get; set; } = new Subject();
        public List<ClientTopicModel> ClientTopics { get; set; } = new List<ClientTopicModel>();
        public List<ClientAssetModel> AssetsInCategory { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AssetsInChildren { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AllAssets { get; set; } = new List<ClientAssetModel>();
    }
}