
namespace TDL.Signals
{
    public class ShowFeedbackPopupFromPuzzleViewSignal : ISignal
    {
        public int Id { get; private set; }

        public ShowFeedbackPopupFromPuzzleViewSignal(int id)
        {
            Id = id;
        }
    }
}