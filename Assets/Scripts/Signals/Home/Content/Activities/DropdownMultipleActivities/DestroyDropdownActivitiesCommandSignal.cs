using TDL.Views;

namespace TDL.Signals
{
    public class DestroyDropdownActivitiesCommandSignal : ISignal
    {
        public AssetItemView AssetItem { get; private set; }

        public DestroyDropdownActivitiesCommandSignal(AssetItemView assetItem)
        {
            AssetItem = assetItem;
        }
    }
}