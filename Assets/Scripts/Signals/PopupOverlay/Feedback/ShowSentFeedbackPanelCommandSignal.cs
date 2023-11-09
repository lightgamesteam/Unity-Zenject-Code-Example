using TDL.Models;

namespace TDL.Signals
{
    public class ShowSentFeedbackPanelCommandSignal : ISignal
    {
        public FeedbackModel.FeedbackType Type { get; private set; }

        public ShowSentFeedbackPanelCommandSignal(FeedbackModel.FeedbackType type)
        {
            Type = type;
        }
    }
}