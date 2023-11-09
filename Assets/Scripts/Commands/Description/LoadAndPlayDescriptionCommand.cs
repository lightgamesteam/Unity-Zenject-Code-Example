using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class LoadAndPlayDescriptionCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;

        private const double NormalSpeakingRate = 1D; 

        public void Execute(ISignal signal)
        {
            var parameter = (LoadAndPlayDescriptionCommandSignal) signal;

            if(parameter.IsShowPopupOverlay)
                _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetSystemTranslations(parameter.CultureCode, LocalizationConstants.LoadingKey)));

            if (IsNeedChangeSpeakingSpeed(parameter.CultureCode))
            {
                _signal.Fire(new AccessibilityTTSPlayCommandSignal(parameter.Description, true, parameter.CultureCode, DescriptionConstants.DescriptionSpeakingRateNorwegian, parameter.AudioSource));
            }
            else
            {
                _signal.Fire(new AccessibilityTTSPlayCommandSignal(parameter.Description, true, parameter.CultureCode, NormalSpeakingRate, parameter.AudioSource));
            }
        }

        private bool IsNeedChangeSpeakingSpeed(string cultureCode)
        {
            var secondPartCultureCode = cultureCode.Substring(cultureCode.Length - 2);
            return secondPartCultureCode.Equals(DescriptionConstants.NorwegianCultureCode);
        }
    }

    public class LoadAndPlayDescriptionCommandSignal : ISignal
    {
        public AudioSource AudioSource { get; }
        public string CultureCode { get; }
        public string Description { get; }
        public bool IsShowPopupOverlay { get; }

        public LoadAndPlayDescriptionCommandSignal(AudioSource audioSource, string cultureCode, string description, bool isShowPopupOverlay = true)
        {
            AudioSource = audioSource;
            CultureCode = cultureCode;
            Description = description;
            IsShowPopupOverlay = isShowPopupOverlay;
        }
    }
}