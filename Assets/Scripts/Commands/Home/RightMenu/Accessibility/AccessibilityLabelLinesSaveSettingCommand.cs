using TDL.Constants;

namespace TDL.Commands
{
    public class AccessibilityLabelLinesSaveSettingCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            var parameter = (AccessibilityLabelLinesClickCommandSignal) signal;
        
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines, parameter.IsEnabled, true);
        }
    }   
}