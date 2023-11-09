
namespace TDL.Signals
{
    public class AccessibilityFontSizeClickViewSignal : ISignal
    {
        public int FontSize { get; private set; }

        public AccessibilityFontSizeClickViewSignal(int fontSize)
        {
            FontSize = fontSize;
        }
    }   
}