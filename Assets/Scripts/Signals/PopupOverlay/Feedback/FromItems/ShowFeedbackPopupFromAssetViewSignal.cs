
namespace TDL.Signals
{
    public class ShowFeedbackPopupFromAssetViewSignal : ISignal
    {
        public int Id { get; private set; }

        public ShowFeedbackPopupFromAssetViewSignal(int id)
        {
            Id = id;
        }
    }
}