
public class ShowLeftMenuCommandSignal : ISignal
{
	public bool IsEnabled { get; private set; }
    
	public ShowLeftMenuCommandSignal(bool isEnabled)
	{
		IsEnabled = isEnabled;
	}
}