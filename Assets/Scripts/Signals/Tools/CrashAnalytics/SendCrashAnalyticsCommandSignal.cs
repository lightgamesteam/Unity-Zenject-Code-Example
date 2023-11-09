
namespace TDL.Signals
{
    public class SendCrashAnalyticsCommandSignal : ISignal
    {
        public string Message { get; private set; }

        public SendCrashAnalyticsCommandSignal(string message)
        {
            Message = message;
        }
    }
}