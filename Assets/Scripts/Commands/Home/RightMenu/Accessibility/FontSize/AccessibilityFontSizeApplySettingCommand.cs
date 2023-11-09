using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class AccessibilityFontSizeApplySettingCommand : ICommandWithParameters
    {
        [Inject] private AccessibilityModel _accessibilityModel;

        private float _mainAppScaler = AccessibilityConstants.FontSizeDefaultScaleFactor;
        private float _modulesAppScaler = AccessibilityConstants.FontSizeDefaultScaleFactor;
        private float _assetItemsAppScaler = AccessibilityConstants.FontSizeDefaultScaleFactor;

        public void Execute(ISignal signal)
        {
            CalculateScalers(signal);
        }
        
        private void CalculateScalers(ISignal signal)
        {
            int prevFontSize;
            
            if (signal is AccessibilityFontSizeClickCommandSignal)
            {
                prevFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityPreviousFontSize);
            }
            else
            {
                prevFontSize = AccessibilityConstants.FontSizeMedium150;
            }

            var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);

            if (currentFontSize != prevFontSize)
            {
                switch (currentFontSize)
                {
                    case AccessibilityConstants.FontSizeSmall100:
                        if (prevFontSize == AccessibilityConstants.FontSizeMedium150)
                        {
                            _mainAppScaler = 1.5f;
                        }
                        else
                        {
                            _mainAppScaler = 2.0f;
                        }
                        
                        _modulesAppScaler = 1.5f;
                        _assetItemsAppScaler = 1.5f;

                        break;
                
                    case AccessibilityConstants.FontSizeMedium150:
                        if (prevFontSize == AccessibilityConstants.FontSizeLarge200)
                        {
                            _mainAppScaler = 1.33f;
                            _assetItemsAppScaler = 1.33f;
                        }
                        else
                        {
                            _mainAppScaler = 0.666f;
                            _assetItemsAppScaler = 0.666f;
                        }
                        
                        _modulesAppScaler = 1.0f;

                        break;
                
                    case AccessibilityConstants.FontSizeLarge200:
                        if (prevFontSize == AccessibilityConstants.FontSizeMedium150)
                        {
                            _mainAppScaler = 0.755f;
                        }
                        else
                        {
                            _mainAppScaler = 0.5f;
                        }
                        
                        _modulesAppScaler = 0.755f;
                        _assetItemsAppScaler = 0.755f;

                        break;
                }   
            }

            _accessibilityModel.MainAppFontSizeScaler = _mainAppScaler;
            _accessibilityModel.ModulesFontSizeScaler = _modulesAppScaler;
            _accessibilityModel.AssetItemsFontSizeScaler = _assetItemsAppScaler;
        }
    }
}