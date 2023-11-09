
namespace TDL.Server
{
    public class FavoriteResponse : ResponseBase
    {
        public ContentRoot ContentRoot { get; set; }
    }

    public class FavoriteRequest
    {
        public int AssetID { get; set; }
    }
}