
namespace TDL.Signals
{
    public class ShowDebugLogCommandSignal : ISignal
    {
        public string Message { get; private set; }

        public ShowDebugLogCommandSignal(string message)
        {
            Message = message;
        }
    }
}
