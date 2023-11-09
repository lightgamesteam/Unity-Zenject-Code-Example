
namespace TDL.Signals
{
    public class CreateDropdownMultipleQuizMobileCommandSignal : ISignal
    {
        public int AssetId { get; private set; }
        public ContentPanel ContentPanel { get; private set; }

        public CreateDropdownMultipleQuizMobileCommandSignal(int assetId, ContentPanel contentPanel)
        {
            AssetId = assetId;
            ContentPanel = contentPanel;
        }
    }
}