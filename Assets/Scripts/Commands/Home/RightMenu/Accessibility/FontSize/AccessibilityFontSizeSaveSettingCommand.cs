using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class AccessibilityFontSizeSaveSettingCommand : ICommandWithParameters
    {
        [Inject] private AccessibilityModel _accessibilityModel;

        public void Execute(ISignal signal)
        {
            var parameter = (AccessibilityFontSizeClickCommandSignal) signal;

            var prevFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
            PlayerPrefsExtension.SetInt(PlayerPrefsKeyConstants.AccessibilityPreviousFontSize, prevFontSize, true);
            PlayerPrefsExtension.SetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize, parameter.FontSize, true);
        }
    }
}