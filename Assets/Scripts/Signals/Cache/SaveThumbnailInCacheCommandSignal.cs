
namespace TDL.Signals
{
    public class SaveThumbnailInCacheCommandSignal : ISignal
    {
        public string ItemId { get; private set; }
        public string ItemName { get; private set; }
        public byte[] DataResponse { get; private set; }

        public SaveThumbnailInCacheCommandSignal(string itemId, string itemName, byte[] dataResponse)
        {
            ItemId = itemId;
            ItemName = itemName;
            DataResponse = dataResponse;
        }
    }
}