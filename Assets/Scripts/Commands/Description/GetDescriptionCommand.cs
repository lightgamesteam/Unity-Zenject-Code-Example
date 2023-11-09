using System;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetDescriptionCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;
        
        public void Execute(ISignal signal)
        {
            var parameter = (GetDescriptionCommandSignal) signal;

            _serverService.GetDescription(parameter.AssetId, parameter.LabelId, parameter.Title, parameter.CultureCode, parameter.DescriptionUrl, parameter.IncludeTeacherAudioFile, parameter.IsMultiViewSecond);
        }
    }

    public class GetDescriptionCommandSignal : ISignal
    {
        public string AssetId { get; }
        public string LabelId { get; }
        public string Title { get; }
        public string CultureCode { get; }
        public string DescriptionUrl { get; }
        public bool IsMultiViewSecond { get; }
        public bool IncludeTeacherAudioFile { get; }

        public GetDescriptionCommandSignal(string assetId, string labelId, string title, string cultureCode, string descriptionUrl, bool includeTeacherAudioFile, bool isMultiViewSecond = false)
        {
            AssetId = assetId;
            LabelId = labelId;
            Title = title;
            CultureCode = cultureCode;
            DescriptionUrl = descriptionUrl;
            IsMultiViewSecond = isMultiViewSecond;
            IncludeTeacherAudioFile = includeTeacherAudioFile;
        }
    }
    
    public class GetLabelDescriptionCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;
        
        public void Execute(ISignal signal)
        {
            var parameter = (GetLabelDescriptionCommandSignal) signal;

            _serverService.GetLabelDescription(parameter.LabelDescriptionUrl, parameter.Callback);
        }
    }
    
    public class GetLabelDescriptionCommandSignal : ISignal
    {
        public string LabelDescriptionUrl { get; }
        public Action<string> Callback { get; }

        public GetLabelDescriptionCommandSignal(string labelDescriptionUrl, Action<string> callback)
        {
            LabelDescriptionUrl = labelDescriptionUrl;
            Callback = callback;
        }
    }
}