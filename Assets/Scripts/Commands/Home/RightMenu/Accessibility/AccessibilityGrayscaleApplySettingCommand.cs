using TDL.Constants;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace TDL.Commands
{
    public class AccessibilityGrayscaleApplySettingCommand : ICommand
    {
        private PostProcessingBehaviour _postProcessing;
        private ColorGradingModel.Settings _settings;
    
        public AccessibilityGrayscaleApplySettingCommand()
        {
            _postProcessing = Camera.main.transform.GetComponent<PostProcessingBehaviour>();
            _settings = _postProcessing.profile.colorGrading.settings;
        }
    
        public void Execute()
        {
            var isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityGrayscale);

            _settings.basic.saturation = isOn ? 0.0f : 1.0f;
            _postProcessing.profile.colorGrading.enabled = isOn;
            _postProcessing.profile.colorGrading.settings = _settings;
        }
    }
}
