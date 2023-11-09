using System.Linq;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class GetLanguageChangedDescriptionViewCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var parameter = (GetLanguageChangedDescriptionViewCommandSignal) signal;

            var labelsToUpdateDescriptions = _homeModel.OpenedDescriptions
                .Where(label => label.Value.IsMultiViewSecond == parameter.IsMultiViewSecond);
            
            foreach (var view in labelsToUpdateDescriptions)
            {
                var assetIdInt = int.Parse(view.Value.AssetId);
                var asset = _contentModel.GetAssetById(assetIdInt);

                var title = string.Empty;
                var descriptionUrl = string.Empty;

                var isLabel = IsLabelDescription(view.Value.LabelId);
                if (isLabel)
                {
                    // label description
                    
                    var labels = asset.AssetDetail.AssetContentPlatform.assetLabel;
                    var labelId = int.Parse(view.Value.LabelId.Split('_')[1]);
                    var label = labels.SingleOrDefault(item => item.labelId == labelId);
                    
                    if (label != null)
                    {
                        title = GetAssociatedContentTranslation(assetIdInt, parameter.CultureCode) + ": " + GetCurrentTranslationsForItem(label.labelLocal.ConvertToLocalName(), parameter.CultureCode);
                        var foundedDescription = label.labelLocal.SingleOrDefault(item => item.Culture.Equals(parameter.CultureCode));

                        if (foundedDescription != null && !string.IsNullOrEmpty(foundedDescription.DescriptionUrl))
                        {
                            descriptionUrl = foundedDescription.DescriptionUrl;
                        }
                    }
                }
                else
                {
                    // asset description
                    
                    title = GetAssociatedContentTranslation(assetIdInt, parameter.CultureCode);
                    var foundedDescription = asset.Asset.LocalizedDescription.SingleOrDefault(item => item.Culture.Equals(parameter.CultureCode));

                    if (foundedDescription != null && !string.IsNullOrEmpty(foundedDescription.DescriptionUrl))
                    {
                        descriptionUrl = foundedDescription.DescriptionUrl;
                    }
                }
                
                
                if (!string.IsNullOrEmpty(descriptionUrl))
                {
                    _serverService.GetLanguageChangedDescription(view.Value.AssetId, view.Value.LabelId, title, parameter.CultureCode, descriptionUrl);   
                }
                else
                {
                    _signal.Fire(new ChangeLanguageDescriptionViewCommandSignal(view.Value.AssetId, view.Value.LabelId, title, parameter.CultureCode, string.Empty));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("No URL for label description | assetId: " + view.Value.AssetId + ", labelId: " + view.Value.LabelId));
                }
            }
        }

        private bool IsLabelDescription(string labelId)
        {
            return !string.IsNullOrEmpty(labelId);
        }
        
        private string GetAssociatedContentTranslation(int assetId, string cultureCode)
        {
            var data = _contentModel.GetAssetById(assetId);
            
            return data.LocalizedName.ContainsKey(cultureCode)
                ? data.LocalizedName[cultureCode]
                : data.LocalizedName[_localizationModel.FallbackCultureCode];
        }
        
        private string GetCurrentTranslationsForItem(LocalName[] itemLocalizedText, string cultureCode)
        {
            var dictionaryLocale = itemLocalizedText.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name);

            var translate = dictionaryLocale.ContainsKey(cultureCode) && !string.IsNullOrEmpty(dictionaryLocale[cultureCode])
                ? dictionaryLocale[cultureCode]
                : dictionaryLocale[_localizationModel.FallbackCultureCode];

            return translate;
        }
    }

    public class GetLanguageChangedDescriptionViewCommandSignal : ISignal
    {
        public string CultureCode { get; }
        public bool IsMultiViewSecond { get; }

        public GetLanguageChangedDescriptionViewCommandSignal(string cultureCode, bool isMultiViewSecond)
        {
            CultureCode = cultureCode;
            IsMultiViewSecond = isMultiViewSecond;
        }
    }
}