
public class SendFeedbackViewSignal : ISignal
{
    public string FeedbackTitle { get; private set; }
    public string FeedbackMessage { get; private set; }
    
    public SendFeedbackViewSignal(string feedbackTitle, string feedbackMessage)
    {
        FeedbackTitle = feedbackTitle;
        FeedbackMessage = feedbackMessage;
    }
}