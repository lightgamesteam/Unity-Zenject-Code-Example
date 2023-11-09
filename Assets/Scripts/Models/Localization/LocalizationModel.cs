using System;
using System.Collections.Generic;
using System.Globalization;
using TDL.Constants;
using TDL.Server;
using UnityEngine;

namespace TDL.Models
{
    public class LocalizationModel
    {
        public Action OnLanguageChanged;

        public List<LanguageResource> AvailableLanguages = new List<LanguageResource>();
        public string FileAvailableLanguages = "AvailableLanguages";

        public Dictionary<string, Dictionary<string, string>> AllSystemTranslations = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, string> CurrentSystemTranslations = new Dictionary<string, string>();

        public string SystemCultureCode { get; private set; } = CultureInfo.CurrentCulture.Name;
        public string FallbackCultureCode { get; private set; } = "en-US";

        public string DefaultCultureCode { get; private set; } = "nb-NO";

        private string _currentLanguageCultureCode;

        public string CurrentLanguageCultureCode
        {
            get => _currentLanguageCultureCode;
            set
            {
                if (_currentLanguageCultureCode == value) return;
                _currentLanguageCultureCode = value;
                OnLanguageChanged?.Invoke();
            }
        }

        public void ForceChangeLanguage()
        {
            OnLanguageChanged?.Invoke();
        }

        public string GetLanguageNameByCultureCode(string cultureCode)
        {
            foreach (var availableLanguage in AvailableLanguages)
            {
                if (cultureCode.Equals(availableLanguage.Culture))
                    return availableLanguage.Name;
            }

            return null;
        }

        public Dictionary<string, string> GetAllTranslationsByKey(string itemKey)
        {
            var result = new Dictionary<string, string>();
            foreach (LanguageResource availableLanguage in AvailableLanguages)
            {
                if(AllSystemTranslations.ContainsKey(availableLanguage.Culture))
                    if(AllSystemTranslations[availableLanguage.Culture].ContainsKey(itemKey))
                        result.Add(availableLanguage.Culture, AllSystemTranslations[availableLanguage.Culture][itemKey]);
            }
            
            return result;
        }

        internal string GetCurrentSystemTranslations(string key)
        {
            return GetSystemTranslations(CurrentLanguageCultureCode, key);
        }

        internal string GetSystemTranslations(string cultureCode, string key)
        {
            var currentSystemTranslations = AllSystemTranslations.ContainsKey(cultureCode)
                ? AllSystemTranslations[cultureCode]
                : AllSystemTranslations.ContainsKey(FallbackCultureCode) ? AllSystemTranslations[FallbackCultureCode] : null;

            if (currentSystemTranslations.TryGetValue(key, out var translate))
            {
                return translate;
            }
            
            return $"Error: Key [{key}] doesn't exist";
        }

        internal string GetLanguageByCultureCode(string cultureCode)
        {
            return AvailableLanguages.Find(al => al.Culture == cultureCode).Name;
        }

        internal string GetCultureCodeByLanguage(string Language)
        {
            return AvailableLanguages.Find(al => al.Name == Language).Culture;
        }
    }
}