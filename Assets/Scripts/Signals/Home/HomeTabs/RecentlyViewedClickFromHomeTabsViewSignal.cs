
namespace TDL.Signals
{
    public class RecentlyViewedClickFromHomeTabsViewSignal : ISignal
    {
        public bool Status { get; private set; }

        public RecentlyViewedClickFromHomeTabsViewSignal(bool status)
        {
            Status = status;
        }
    }
}