
public class AccessibilityGrayscaleClickCommandSignal : ISignal
{
    public bool IsEnabled { get; private set; }
    
    public AccessibilityGrayscaleClickCommandSignal(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}