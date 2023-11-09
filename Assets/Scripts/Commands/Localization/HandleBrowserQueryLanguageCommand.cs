using Extension;
using TDL.Constants;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{

    public class HandleBrowserQueryLanguageCommand : ICommand
    {
        private const string PARAMETER_LANG = "lang";
        [Inject] private SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;
        
        public void Execute()
        {
#if  UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("Handling browser query");
            var lang = BrowserExtensions.GetQueryParameter(PARAMETER_LANG);
            Debug.Log($"lang is: {lang}");
            if (lang == "null")
                return;
            
            if(_localizationModel.AvailableLanguages.Exists(x => x.Culture == lang))
            {
                _localizationModel.CurrentLanguageCultureCode = lang;
                PlayerPrefs.SetString(LocalizationConstants.ChosenLanguage, _localizationModel.CurrentLanguageCultureCode);
            }
#endif
        }

    }
}