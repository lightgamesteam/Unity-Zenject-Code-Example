using UnityEngine;

namespace TDL.Signals
{
    public class LoadAndPlayDescriptionViewSignal : ISignal
    {
        public string Id { get; }
        public AudioSource AudioSource { get; }
        public string CultureCode { get; }
        public string Description { get; }

        public LoadAndPlayDescriptionViewSignal(string id, AudioSource audioSource, string cultureCode, string description)
        {
            Id = id;
            AudioSource = audioSource;
            CultureCode = cultureCode;
            Description = description;
        }
    }   
}