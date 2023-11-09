using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class AccessibilityGrayscaleSaveSettingCommand : ICommandWithParameters
    {
        [Inject] private AccessibilityModel _accessibilityModel;

        public void Execute(ISignal signal)
        {
            var parameter = (AccessibilityGrayscaleClickCommandSignal) signal;
        
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityGrayscale, parameter.IsEnabled, true);
            _accessibilityModel.GrayscaleActivated = parameter.IsEnabled;
        }
    }   
}