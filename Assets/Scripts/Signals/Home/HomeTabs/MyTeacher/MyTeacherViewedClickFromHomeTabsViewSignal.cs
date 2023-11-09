
namespace TDL.Signals
{
    public class MyTeacherViewedClickFromHomeTabsViewSignal : ISignal
    {
        public bool Status { get; private set; }

        public MyTeacherViewedClickFromHomeTabsViewSignal(bool status)
        {
            Status = status;
        }
    }
}