using TDL.Models;

namespace TDL.Signals
{
    public class ShowMainFeedbackPanelCommandSignal : ISignal
    {
        public int Id { get; private set; }
        public FeedbackModel.FeedbackType Type { get; private set; }

        public ShowMainFeedbackPanelCommandSignal(int id, FeedbackModel.FeedbackType type)
        {
            Id = id;
            Type = type;
        }
    }
}