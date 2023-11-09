using TDL.Constants;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SetChosenLanguageCommand : ICommand
    {
        [Inject] private LocalizationModel _localizationModel;

        public void Execute()
        {
            var chosenLanguage = PlayerPrefs.GetString(LocalizationConstants.ChosenLanguage, string.Empty);

            if (IsChosenLanguageExistsInApp(chosenLanguage))
            {
                SetCurrentSystemTranslations(chosenLanguage);
                SetCurrentLanguage(chosenLanguage);
            }
            else
            {
                SaveDefaultLanguageToCache();
                SetCurrentSystemTranslations(_localizationModel.DefaultCultureCode);
                SetCurrentLanguage(_localizationModel.DefaultCultureCode);
            }
        }

        private void SaveDefaultLanguageToCache()
        {
            PlayerPrefs.SetString(LocalizationConstants.ChosenLanguage, _localizationModel.DefaultCultureCode);
        }

        private void SetCurrentSystemTranslations(string chosenLanguage)
        {
            _localizationModel.CurrentSystemTranslations = _localizationModel.AllSystemTranslations[chosenLanguage];
        }

        private void SetCurrentLanguage(string chosenLanguage)
        {
            _localizationModel.CurrentLanguageCultureCode = chosenLanguage;
        }

        private bool IsChosenLanguageExistsInApp(string chosenCultureCode)
        {
            return !string.IsNullOrEmpty(chosenCultureCode)
                   && _localizationModel.AllSystemTranslations.ContainsKey(chosenCultureCode);
        }
    }
}