using TMPro;

namespace TDL.Core
{
    public interface IFontSizeView
    {
        TextMeshProUGUI Title { get; }
        float DefaultFontSize { get; set; }
    }   
}