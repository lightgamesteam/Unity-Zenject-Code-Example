using System.Text.RegularExpressions;
using TDL.Constants;

namespace TDL.Commands
{
    public class AccessibilityTTSPlayCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            if (DeviceInfo.IsPCInterface())
            {
                var isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio);
                var parameter = (AccessibilityTTSPlayCommandSignal) signal;

                if (isOn || parameter.ForcePlay)
                {
                    //SynthesizeController.Instance.Synthesize(GetTextWithoutHTML(parameter.Text), parameter.AudioSource, parameter.Language, parameter.SpeakingRate, parameter.SsmlToggle);
                }
            }
        }

        private string GetTextWithoutHTML(string textToSpeech)
        {
            return Regex.Replace(textToSpeech, "<.*?>", string.Empty);
        }
    }
}