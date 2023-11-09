
namespace TDL.Signals
{
	public class RightMenuClickViewSignal : ISignal
	{
		public bool IsEnabled { get; private set; }
    
		public RightMenuClickViewSignal(bool isEnabled)
		{
			IsEnabled = isEnabled;
		}
	}
}