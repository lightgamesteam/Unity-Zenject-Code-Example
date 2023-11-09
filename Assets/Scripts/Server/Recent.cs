
namespace TDL.Server
{
    public class RecentResponse : ResponseBase
    {
        public ContentRoot ContentRoot { get; set; }
    }

    public class RecentRequest
    {
        public int AssetID { get; set; }
    }
}