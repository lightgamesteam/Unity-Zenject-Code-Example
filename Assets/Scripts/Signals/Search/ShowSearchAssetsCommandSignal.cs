
namespace TDL.Signals
{
    public class ShowSearchAssetsCommandSignal : ISignal
    {
        public int[] FoundedAssetIds { get; private set; }

        public ShowSearchAssetsCommandSignal(int[] foundedAssetIds)
        {
            FoundedAssetIds = foundedAssetIds;
        }
    }
}