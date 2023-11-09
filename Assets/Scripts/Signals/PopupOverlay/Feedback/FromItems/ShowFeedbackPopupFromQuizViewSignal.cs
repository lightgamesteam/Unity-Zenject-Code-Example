
namespace TDL.Signals
{
    public class ShowFeedbackPopupFromQuizViewSignal : ISignal
    {
        public int Id { get; private set; }

        public ShowFeedbackPopupFromQuizViewSignal(int id)
        {
            Id = id;
        }
    }
}