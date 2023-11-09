using TDL.Views;

namespace TDL.Signals
{
    public class OnDropdownMultiplePuzzleClickViewSignal : ISignal
    {
        public AssetItemView AssetItem { get; private set; }

        public OnDropdownMultiplePuzzleClickViewSignal(AssetItemView assetItem)
        {
            AssetItem = assetItem;
        }
    }
}
