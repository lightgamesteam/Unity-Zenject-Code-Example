
public class StartDownloadAssetCommandSignal : ISignal
{
    public int Id { get; private set; }
    public  bool UpdateAssetItemDownloadStatus { get; private set; }

    public StartDownloadAssetCommandSignal(int id, bool updateAssetItemDownloadStatus = true)
    {
        Id = id;
        UpdateAssetItemDownloadStatus = updateAssetItemDownloadStatus;
    }
}