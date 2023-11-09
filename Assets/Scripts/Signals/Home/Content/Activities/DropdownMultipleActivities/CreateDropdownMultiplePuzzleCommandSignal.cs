using TDL.Views;

namespace TDL.Signals
{
    public class CreateDropdownMultiplePuzzleCommandSignal : ISignal
    {
        public AssetItemView AssetItem { get; private set; }

        public CreateDropdownMultiplePuzzleCommandSignal(AssetItemView assetItem)
        {
            AssetItem = assetItem;
        }
    }
}