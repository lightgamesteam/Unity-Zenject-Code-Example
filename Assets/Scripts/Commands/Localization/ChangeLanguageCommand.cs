using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class ChangeLanguageCommand : ICommandWithParameters
    {
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (ChangeLanguageCommandSignal) signal;

            var chosenLanguage = _localizationModel.AvailableLanguages[parameter.LanguageIndex];

            if (IsChosenLanguageExistsInApp(chosenLanguage.Culture))
            {
                if (ShouldChangeLanguage(chosenLanguage.Culture))
                {
                    SaveCurrentLanguageToCache(chosenLanguage.Culture);
                    SetCurrentSystemTranslations(chosenLanguage.Culture);
                    SetCurrentLanguage(chosenLanguage.Culture);
                }
            }
            else
            {
                SetLanguageIsNotSupported();
            }
        }

        private void SetLanguageIsNotSupported()
        {
            _localizationModel.ForceChangeLanguage();
            _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LanguageNotSupportedKey), false, PopupOverlayType.MessageBox));
        }

        private bool IsChosenLanguageExistsInApp(string chosenCultureCode)
        {
            return _localizationModel.AllSystemTranslations.ContainsKey(chosenCultureCode);
        }

        private bool ShouldChangeLanguage(string chosenCultureCode)
        {
            return !chosenCultureCode.Equals(_localizationModel.CurrentLanguageCultureCode);
        }

        private void SaveCurrentLanguageToCache(string chosenLanguage)
        {
            PlayerPrefs.SetString(LocalizationConstants.ChosenLanguage, chosenLanguage);
        }

        private void SetCurrentSystemTranslations(string chosenLanguage)
        {
            _localizationModel.CurrentSystemTranslations = _localizationModel.AllSystemTranslations[chosenLanguage];
        }

        private void SetCurrentLanguage(string chosenLanguage)
        {
            _localizationModel.CurrentLanguageCultureCode = chosenLanguage;
        }
    }
}