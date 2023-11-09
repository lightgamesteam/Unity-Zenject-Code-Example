
public class UpdateDownloadProgressCommandSignal : ISignal
{
    public int ItemId { get; private set; }
    public long Downloaded { get; private set; }
    public long Length { get; private set; }

    public UpdateDownloadProgressCommandSignal(int itemId, long downloaded, long length)
    {
        ItemId = itemId;
        Downloaded = downloaded;
        Length = length;
    }
}