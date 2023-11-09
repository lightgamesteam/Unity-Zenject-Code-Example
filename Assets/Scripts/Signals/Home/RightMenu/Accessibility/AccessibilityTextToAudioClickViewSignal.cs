
namespace TDL.Signals
{
    public class AccessibilityTextToAudioClickViewSignal : ISignal
    {
        public bool IsEnabled { get; private set; }
    
        public AccessibilityTextToAudioClickViewSignal(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }   
}