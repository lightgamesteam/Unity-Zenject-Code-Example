using TDL.Constants;

namespace TDL.Commands
{
    public class AccessibilityTextToAudioSaveSettingCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            var parameter = (AccessibilityTextToAudioClickCommandSignal) signal;
        
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio, parameter.IsEnabled, true);
        }
    }   
}