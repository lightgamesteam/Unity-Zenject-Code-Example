using TDL.Views;

namespace TDL.Signals
{
    public class OnDropdownMultipleQuizClickViewSignal : ISignal
    {
        public AssetItemView AssetItem { get; private set; }

        public OnDropdownMultipleQuizClickViewSignal(AssetItemView assetItem)
        {
            AssetItem = assetItem;
        }
    }
}