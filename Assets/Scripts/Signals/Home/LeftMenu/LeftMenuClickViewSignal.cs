
public class LeftMenuClickViewSignal : ISignal
{
    public bool IsEnabled { get; private set; }
    
    public LeftMenuClickViewSignal(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}