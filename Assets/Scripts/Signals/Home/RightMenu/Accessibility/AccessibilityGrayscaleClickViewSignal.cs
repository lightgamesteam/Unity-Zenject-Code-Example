
public class AccessibilityGrayscaleClickViewSignal : ISignal
{
    public bool IsEnabled { get; private set; }
    
    public AccessibilityGrayscaleClickViewSignal(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}