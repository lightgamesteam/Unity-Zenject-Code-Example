using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class HideMainFeedbackPanelCommand : ICommand
    {
        [Inject] private FeedbackModel _feedbackModel;

        public void Execute()
        {
            _feedbackModel.IsMainFeedbackActive = false;
        }
    }
}