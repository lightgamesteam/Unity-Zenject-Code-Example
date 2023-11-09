
namespace TDL.Signals
{
    public class MyContentViewedClickFromHomeTabsViewSignal : ISignal
    {
        public bool Status { get; private set; }

        public MyContentViewedClickFromHomeTabsViewSignal(bool status)
        {
            Status = status;
        }
    }
}