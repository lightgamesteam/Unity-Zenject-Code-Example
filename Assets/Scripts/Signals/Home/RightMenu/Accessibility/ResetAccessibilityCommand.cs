using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ResetAccessibilityCommand : ICommand
    {
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        public void Execute()
        {
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio, false, true);
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityGrayscale, false, true);
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines, false, true);
            PlayerPrefsExtension.SetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize, AccessibilityConstants.FontSizeMedium150, true);
            
            _accessibilityModel.MainAppFontSizeScaler = AccessibilityConstants.FontSizeDefaultScaleFactor;
        }
    }   
}