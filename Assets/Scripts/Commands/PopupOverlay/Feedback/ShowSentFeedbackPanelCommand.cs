using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ShowSentFeedbackPanelCommand : ICommand
    {
        [Inject] private FeedbackModel _feedbackModel;

        public void Execute()
        {
            _feedbackModel.IsSentFeedbackActive = true;
        }
    }
}