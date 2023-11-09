using UnityEngine;

namespace TDL.Commands
{
    public class AccessibilityTTSPlayCommandSignal : ISignal
    {
        public string Text { get; }
        public bool ForcePlay { get; }
        public string Language { get; }
        public double SpeakingRate { get; }
        public AudioSource AudioSource { get; }
        public bool SsmlToggle { get; }

        public AccessibilityTTSPlayCommandSignal(string text, bool forcePlay = false, string language = "", double speakingRate = 1, AudioSource audioSource = null, bool ssmlToggle = false)
        {
            Text = text;
            ForcePlay = forcePlay;
            Language = language;
            SpeakingRate = speakingRate;
            AudioSource = audioSource;
            SsmlToggle = ssmlToggle;
        }
    }   
}