using System;
using TDL.Constants;

namespace TDL.Models
{
    public class AccessibilityModel
    {
        public Action<bool> OnGrayscaleChanged;
        public Action<float> OnFontSizeChanged;

        public bool UpdateLabelLines;

        private bool _grayscaleActivated;
        public bool GrayscaleActivated
        {
            get { return _grayscaleActivated; }
            set
            {
                if (_grayscaleActivated == value) return;
                _grayscaleActivated = value;
                OnGrayscaleChanged?.Invoke(_grayscaleActivated);
            }
        }

        private float _mainAppFontSizeScaler;
        public float MainAppFontSizeScaler
        {
            get { return _mainAppFontSizeScaler; }
            set
            {
                if (_mainAppFontSizeScaler == value) return;
                _mainAppFontSizeScaler = value;
                OnFontSizeChanged?.Invoke(_mainAppFontSizeScaler);
            }
        }

        public float ModulesFontSizeScaler { get; set; }
        public float AssetItemsFontSizeScaler { get; set; }

        public bool IsActiveTextToAudio => PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio);
        public void SetActiveTextToAudio(bool isOn)
        {
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio, isOn);
        }
    }   
}