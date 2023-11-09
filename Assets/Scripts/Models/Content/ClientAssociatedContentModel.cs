using TDL.Server;

namespace TDL.Models
{
    public class ClientAssociatedContentModel
    {
        public AssociatedAsset AssociatedContent { get; set; }
        public ClientAssetModel ParentAsset { get; set; }
    }
}