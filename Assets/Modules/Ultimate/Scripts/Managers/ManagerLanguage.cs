using System;
using System.Collections.Generic;
using System.Linq;
using Module;
using TDL.Models;
using TDL.Server;
using Zenject;

namespace TDL.Modules.Ultimate.Core.Managers {
    public class ManagerLanguage : ManagerBase, ILanguageListeners, ILanguageHandler {
        [Inject] private readonly LocalizationModel _data = default;
        private Dictionary<int, LanguageResource> _languageByGuid;
        
        #region ILanguageListeners
        
        public event Action<ILanguageHandler> LocalizeEvent;
        public event Action<ILanguageHandler> LastLocalizeEvent;
        
        public void Invoke(Action<ILanguageHandler> action) {
            action.Invoke(this);
        }

        #endregion
        
        #region ILanguageHandler
        
        public void SetLanguage(int id) {
            if (_languageByGuid.TryGetValue(id, out var language)) {
                _data.CurrentLanguageCultureCode = language.Culture;
                LocalizeEvent?.Invoke(this);
                LastLocalizeEvent?.Invoke(this);
            }
        }
        
        public string GetCurrentName() {
            foreach (var languageResource in _languageByGuid) {
                if (languageResource.Value.Culture.Equals(_data.CurrentLanguageCultureCode)) {
                    return languageResource.Value.Name;
                }
            }
            return "No translation";
        }
        
        public string GetCurrentCulture() {
            return _data.CurrentLanguageCultureCode;
        }
        
        public string GetCurrentTranslations(string key) {
            if (_data.AllSystemTranslations.ContainsKey(_data.CurrentLanguageCultureCode)) {
                var currentLanguage = _data.AllSystemTranslations[_data.CurrentLanguageCultureCode];
                if (!currentLanguage.ContainsKey(key)) {
                    this.LogWarningRed(key, $"No key in '{_data.CurrentLanguageCultureCode}' language");
                } else {
                    return currentLanguage[key];
                }
            }
            
            var fallbackLanguage = _data.AllSystemTranslations[_data.FallbackCultureCode];
            if (fallbackLanguage.ContainsKey(key)) {
                return fallbackLanguage[key];
            }
            this.LogErrorRed(key, "No key in any language");
            return "No translation";
        }

        public string GetCurrentTranslations(IEnumerable<LocalName> localNameArray) {
            var dictionaryLocale = localNameArray.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name);
            var cultureCode = _data.CurrentLanguageCultureCode;
            var translate = dictionaryLocale.ContainsKey(cultureCode) && !string.IsNullOrEmpty(dictionaryLocale[cultureCode])
                ? dictionaryLocale[cultureCode]
                : dictionaryLocale[_data.FallbackCultureCode];
            return translate;
        }
        
        public LocalName[] GetAllTranslations(string key) {
            var result = new List<LocalName>();
            foreach (var cultureDictionary in _data.AllSystemTranslations) {
                if (cultureDictionary.Value.ContainsKey(key)) {
                    result.Add(new LocalName {
                        Culture = cultureDictionary.Key,
                        Name = cultureDictionary.Value[key]
                    });
                }
            }
            if (result.Count == 0) {
                this.LogErrorRed(key, "No key in any language");
                result.Add(new LocalName {
                    Culture = "en-US",
                    Name = "No translation"
                });
            }
            return result.ToArray();
        }
        
        public IEnumerable<LanguageResource> GetLanguageResources() {
            return _languageByGuid.Values.ToArray();
        }
        
        #endregion

        #region Protected methods
        
        protected override void Construct(DiContainer container) {
            base.Construct(container);
            _languageByGuid = CreateLanguageByGuid(_data.AvailableLanguages);
        }
        
        #endregion
        
        #region Private methods

        private static Dictionary<int, LanguageResource> CreateLanguageByGuid(IEnumerable<LanguageResource> languageArray) {
            var result = new Dictionary<int, LanguageResource>();
            foreach (var language in languageArray) {
                if (!result.ContainsKey(language.Id)) {
                    result.Add(language.Id, language);
                } else {
                    Debug.LogWarning(language.Id + " is contains");
                }
            }
            return result;
        }
        
        #endregion
    }
    
    public interface ILanguageListeners {
        event Action<ILanguageHandler> LocalizeEvent;
        event Action<ILanguageHandler> LastLocalizeEvent;
        void Invoke(Action<ILanguageHandler> action);
    }
    
    public interface ILanguageHandler {
        void SetLanguage(int id);
        string GetCurrentName();
        string GetCurrentCulture();
        string GetCurrentTranslations(string key);
        string GetCurrentTranslations(IEnumerable<LocalName> localNameArray);
        LocalName[] GetAllTranslations(string key);
        IEnumerable<LanguageResource> GetLanguageResources();
    }
}