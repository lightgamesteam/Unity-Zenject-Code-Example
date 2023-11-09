using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class TextToSpeechItemWithNoText : MonoBehaviour
    {
        [SerializeField] protected string _key;
        [Inject] protected LocalizationModel _localizationModel;

        public virtual string GetTranslation()
        {
            if (_localizationModel.CurrentSystemTranslations.ContainsKey(_key))
            {
                return _localizationModel.CurrentSystemTranslations[_key];
            }
        
            return string.Empty;
        }
    }
}