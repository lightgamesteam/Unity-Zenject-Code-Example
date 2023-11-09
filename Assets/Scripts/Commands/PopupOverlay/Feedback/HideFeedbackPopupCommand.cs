using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class HideFeedbackPopupCommand : ICommand
    {
        [Inject] private FeedbackModel _feedbackModel;

        public void Execute()
        {
            _feedbackModel.Title = string.Empty;
            _feedbackModel.IsMainFeedbackActive = false;
            _feedbackModel.IsSentFeedbackActive = false;
        }
    }
}