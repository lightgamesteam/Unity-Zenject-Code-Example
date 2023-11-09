using TDL.Views;

namespace TDL.Signals
{
    public class CreateDropdownMultipleQuizCommandSignal : ISignal
    {
        public AssetItemView AssetItem { get; private set; }

        public CreateDropdownMultipleQuizCommandSignal(AssetItemView assetItem)
        {
            AssetItem = assetItem;
        }
    }
}