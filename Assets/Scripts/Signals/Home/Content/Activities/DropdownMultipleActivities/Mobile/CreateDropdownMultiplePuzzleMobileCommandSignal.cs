
namespace TDL.Signals
{
    public class CreateDropdownMultiplePuzzleMobileCommandSignal : ISignal
    {
        public int AssetId { get; private set; }
        public ContentPanel ContentPanel { get; private set; }

        public CreateDropdownMultiplePuzzleMobileCommandSignal(int assetId, ContentPanel contentPanel)
        {
            AssetId = assetId;
            ContentPanel = contentPanel;
        }
    }
}