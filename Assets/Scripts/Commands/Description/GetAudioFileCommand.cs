using System;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class GetAudioFileCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (GetAudioFileCommandSignal) signal;

            _serverService.GetAudioFile(parameter.AudioFileUrl, parameter.Callback);
        }
    }

    public class GetAudioFileCommandSignal : ISignal
    {
        public string AudioFileUrl { get; }

        public Action<AudioClip> Callback;


        public GetAudioFileCommandSignal(string audioFileUrl, Action<AudioClip> callback)
        {
            AudioFileUrl = audioFileUrl;
            Callback = callback;
        }
        
    }
}