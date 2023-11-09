
namespace TDL.Signals
{
	public class SetHomeTabsVisibilityViewSignal : ISignal
	{
		public bool Status { get; private set; }
    
		public SetHomeTabsVisibilityViewSignal(bool status)
		{
			Status = status;
		}
	}
}