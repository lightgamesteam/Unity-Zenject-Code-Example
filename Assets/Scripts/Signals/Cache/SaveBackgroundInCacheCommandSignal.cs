
public class SaveBackgroundInCacheCommandSignal : ISignal
{
    public string ItemId { get; private set; }
    public string ItemName { get; private set; }
    public byte[] DataResponse { get; private set; }

    public SaveBackgroundInCacheCommandSignal(string itemId, string itemName, byte[] dataResponse)
    {
        ItemId = itemId;
        ItemName = itemName;
        DataResponse = dataResponse;
    }
}