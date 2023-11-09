
namespace TDL.Signals
{
	public class ShowRightMenuCommandSignal : ISignal
	{
		public bool IsEnabled { get; private set; }
    
		public ShowRightMenuCommandSignal(bool isEnabled)
		{
			IsEnabled = isEnabled;
		}
	}
}