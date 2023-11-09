using System.Collections.Generic;
using TDL.Core;
using TDL.Server;

namespace TDL.Models
{
    public class ClientGradeModel : ICategory
    {
        public int Id { get; set; }
        public Grade Grade { get; set; }
        public List<ClientSubjectModel> ClientSubjects { get; set; } = new List<ClientSubjectModel>();
        public List<ClientAssetModel> AssetsInCategory { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AssetsInChildren { get; set; } = new List<ClientAssetModel>();
        public List<ClientAssetModel> AllAssets { get; set; } = new List<ClientAssetModel>();
    }
}