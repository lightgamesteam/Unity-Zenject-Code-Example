using TDL.Views;

namespace Signals
{
    public class DownloadThumbnailByIdSignal : ISignal
    {
        public AssetItemView Asset { get; set; }

        public DownloadThumbnailByIdSignal(AssetItemView assset)
        {
            Asset = assset;
        }
    }
}