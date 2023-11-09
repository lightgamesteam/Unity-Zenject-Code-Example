using System;
using TDL.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    [RequireComponent(typeof(Slider))]
    public class TextToSpeechSlider : TextToSpeechItemWithNoText
    {
        private Slider _slider;

        private void Awake()
        {
            if (_slider == null)
                _slider = GetComponent<Slider>();
        }

        public override string GetTranslation()
        {
            if (_localizationModel.CurrentSystemTranslations.ContainsKey(_key))
            {
                return $"{_localizationModel.CurrentSystemTranslations[_key]} {Math.Round(_slider.value, 1).ToString().Replace(',', '.')}";
            }
        
            return string.Empty;
        }
    }
}