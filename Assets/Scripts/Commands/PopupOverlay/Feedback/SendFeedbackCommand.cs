using System.Text.RegularExpressions;
using TDL.Models;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class SendFeedbackCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;
        [Inject] private ContentModel _contentModel;
        [Inject] private FeedbackModel _feedbackModel;

        private const string FeedbackStatus = "New";

        public void Execute(ISignal signal)
        {
            var parameter = (SendFeedbackCommandSignal) signal;

            var feedbackRequest = new FeedbackModel.FeedbackRequest();
            feedbackRequest.AssetContentId = _feedbackModel.AssetContentId.ToString();
            feedbackRequest.Message = ReplaceNewLineWithHtmlTag(parameter.Message);

            feedbackRequest.GradeId = _contentModel.SelectedGrade != null ? _contentModel.SelectedGrade.Id.ToString() : (-1).ToString();
            feedbackRequest.SubjectId = _contentModel.SelectedSubject != null ? _contentModel.SelectedSubject.Id.ToString() : (-1).ToString();
            feedbackRequest.TopicId = _contentModel.SelectedTopic != null ? _contentModel.SelectedTopic.Id.ToString() : (-1).ToString();
            feedbackRequest.SubtopicId = _contentModel.SelectedSubtopic != null ? _contentModel.SelectedSubtopic.Id.ToString() : (-1).ToString();

            feedbackRequest.Status = FeedbackStatus;

            _serverService.SendFeedback(feedbackRequest);
        }

        private string ReplaceNewLineWithHtmlTag(string message)
        {
            return Regex.Replace(message, @"\r\n?|\n", "<br />");
        }
    }
}