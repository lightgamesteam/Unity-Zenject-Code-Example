using System.Linq;
using TDL.Constants;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class ChangeLanguageDescriptionViewCommand : ICommandWithParameters
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            var parameter = (ChangeLanguageDescriptionViewCommandSignal) signal;

            var id = string.IsNullOrEmpty(parameter.LabelId) ? parameter.AssetId : parameter.LabelId;
            
            var view = _homeModel.OpenedDescriptions.Single(item => item.Key == id).Value;
            view.CultureCode = parameter.CultureCode;
            view.SetTitle(parameter.Title);
            
            if (string.IsNullOrEmpty(parameter.Description))
            {
                view.SetNoTranslationText(_localizationModel.GetSystemTranslations(parameter.CultureCode, LocalizationConstants.LabelDescriptionNoTranslationKey));
            }
            else
            {
                view.SetDescription(parameter.Description);
            }
            
            UpdateTooltips(view.gameObject, parameter.CultureCode);
        }

        private void UpdateTooltips(GameObject go, string cultureCode)
        {
            go.gameObject.GetAllComponentsInChildren<TooltipEvents>().ForEach(t =>
            {
                var keyTranslation = t.GetKey();

                if (!string.IsNullOrEmpty(keyTranslation))
                {
                    t.SetHint(_localizationModel.GetSystemTranslations(cultureCode, keyTranslation));
                }
            });
        }
    }  
    
    public class ChangeLanguageDescriptionViewCommandSignal : ISignal
    {
        public string AssetId { get; }
        public string LabelId { get; }
        public string Title { get; }
        public string CultureCode { get; }
        public string Description { get; }

        public ChangeLanguageDescriptionViewCommandSignal(string assetId, string labelId, string title, string cultureCode, string description)
        {
            AssetId = assetId;
            LabelId = labelId;
            Title = title;
            CultureCode = cultureCode;
            Description = description;
        }
    }
}