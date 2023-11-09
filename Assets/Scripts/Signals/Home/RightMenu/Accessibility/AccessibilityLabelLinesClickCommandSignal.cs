
public class AccessibilityLabelLinesClickCommandSignal : ISignal
{
    public bool IsEnabled { get; private set; }
    
    public AccessibilityLabelLinesClickCommandSignal(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}