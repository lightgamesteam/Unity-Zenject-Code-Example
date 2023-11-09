
public class AccessibilityLabelLinesClickViewSignal : ISignal
{
    public bool IsEnabled { get; private set; }
    
    public AccessibilityLabelLinesClickViewSignal(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}