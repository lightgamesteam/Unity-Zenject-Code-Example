
namespace TDL.Signals
{
    public class CheckInternetConnectionCommandSignal : ISignal
    {
        public ISignal SignalSource { get; private set; }

        public CheckInternetConnectionCommandSignal(ISignal signalSource)
        {
            SignalSource = signalSource;
        }
    }
}