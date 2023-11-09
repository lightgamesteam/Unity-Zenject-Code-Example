
namespace TDL.Signals
{
    public class AccessibilityFontSizeClickCommandSignal : ISignal
    {
        public int FontSize { get; private set; }

        public AccessibilityFontSizeClickCommandSignal(int fontSize)
        {
            FontSize = fontSize;
        }
    }   
}