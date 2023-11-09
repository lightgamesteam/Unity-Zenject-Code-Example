
public class AccessibilityTextToAudioClickCommandSignal : ISignal
{
    public bool IsEnabled { get; private set; }
    
    public AccessibilityTextToAudioClickCommandSignal(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}