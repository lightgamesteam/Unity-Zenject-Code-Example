
public class SendFeedbackCommandSignal : ISignal
{
    public int Id { get; private set; }
    public string Message { get; private set; }

    public SendFeedbackCommandSignal(int id, string message)
    {
        Id = id;
        Message = message;
    }
}