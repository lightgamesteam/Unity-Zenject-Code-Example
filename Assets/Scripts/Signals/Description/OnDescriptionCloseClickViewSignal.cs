
namespace TDL.Signals
{
    public class OnDescriptionCloseClickViewSignal : ISignal
    {
        public string AssetId { get; }
        public string LabelId { get; }

        public OnDescriptionCloseClickViewSignal(string assetId, string labelId)
        {
            AssetId = assetId;
            LabelId = labelId;
        }
    } 
    
    public class CloseDescriptionViewSignal : ISignal
    {

    }
}