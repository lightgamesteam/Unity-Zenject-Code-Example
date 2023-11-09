
namespace TDL.Signals
{
    public class CreateActivityContentCommandSignal : ISignal
    {
        public string ActivityName { get; private set; }
        public string ContentResponse { get; private set; }

        public CreateActivityContentCommandSignal(string activityName, string contentResponse)
        {
            ActivityName = activityName;
            ContentResponse = contentResponse;
        }
    }
}