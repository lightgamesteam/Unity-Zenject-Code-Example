
public class UpdateHomeTabsStatusViewSignal : ISignal
{
	public bool Status { get; private set; }
    
	public UpdateHomeTabsStatusViewSignal(bool status)
	{
		Status = status;
	}
}