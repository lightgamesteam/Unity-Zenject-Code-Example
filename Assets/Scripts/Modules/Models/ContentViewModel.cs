using System;
using System.Collections.Generic;
using System.Linq;
using TDL.Constants;
using TMPro;
using UnityEngine;
using Zenject;

namespace TDL.Models
{
    public class ContentViewModel
    {
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private ApplicationSettingsInstaller.AssetItemResources _assetItemResources;

        public ContentHomeView contentHomeView;
        public ContentViewPC contentViewPC;
        public ContentViewMobile contentViewMobile;
        public Transform ContentMain { get; set; }
        
        public int mainAssetID = -1;
        public int secondAssetID = -1;
   
        public static Action<(int layer, string cultureCode)> OnChangeLayerLanguage = delegate {  };
   
        private readonly Dictionary<int, string> cultureCodeOnLayer = new Dictionary<int, string>();
        
        public List<Texture2D> CachedTextures = new List<Texture2D>();
   
        private string _currentContentViewCultureCode;
        internal string CurrentContentViewCultureCode
        {
            get
            {
                if (String.IsNullOrEmpty(_currentContentViewCultureCode))
                    return _localizationModel.CurrentLanguageCultureCode;
                    
                return _currentContentViewCultureCode;
            }
            set => _currentContentViewCultureCode = value;
        }

        public void SetLanguageOnLayer(int layer, string cultureCode, bool isMainLanguage = false)
        {
            if(isMainLanguage)
                CurrentContentViewCultureCode = cultureCode;

            cultureCodeOnLayer[layer] = cultureCode;
      
            TranslateLayers();
            OnChangeLayerLanguage.Invoke((layer, cultureCode));
        }
   
        public string GetLanguageOnLayer(int layer)
        {
            if (cultureCodeOnLayer.ContainsKey(layer))
            {
                return cultureCodeOnLayer[layer];
            }

            return CurrentContentViewCultureCode;
        }
        
        internal void UpdateCurrentLanguage()
        {
            CurrentContentViewCultureCode = _localizationModel.CurrentLanguageCultureCode;
        }
   
        internal string GetCurrentSystemTranslations(string key)
        {
            return _localizationModel.GetSystemTranslations(CurrentContentViewCultureCode, key);
        }
   
        internal string GetSystemTranslations(string cultureCode, string key)
        {
            return _localizationModel.GetSystemTranslations(cultureCode, key);
        }

        internal Sprite GetSpriteType(string type)
        {
            switch (type.ToLower())
            {
                case AssetTypeConstants.Type_3D_Model:
                    return _assetItemResources.Type3DModel;

                case AssetTypeConstants.Type_3D_Video:
                    return _assetItemResources.Type3DVideo;

                case AssetTypeConstants.Type_2D_Video:
                    return _assetItemResources.Type2DVideo;
                
                case AssetTypeConstants.Type_Multilayered:
                    return _assetItemResources.TypeMultilayered;
                
                case AssetTypeConstants.Type_360_Model:
                    return _assetItemResources.Type360Model;
                
                case AssetTypeConstants.Type_Rigged_Model:
                    return _assetItemResources.TypeRiggedModel;

                case AssetTypeConstants.Type_Module:
                    return _assetItemResources.TypeModule;

                default:
                    return null;
            }
        }
        
        public void ClearCachedTextures()
        {            
            foreach (var texture in CachedTextures)
            {
                // ToDo destroy after each frame
                GameObject.Destroy(texture);
            }

            if (CachedTextures.Count > 0)
            {
                CachedTextures.Clear();
            }
        }

        #region Translator
        // TODO : Create commands 
        private void TranslateLayers()
        {
            TranslateAllTooltips();
            TranslateAllSystemTexts();
            TranslateAllLabelTexts();
        }
        
        // Label Text -------------------------------------------------------
        List<(int assetId, int labelId, TextMeshProUGUI listText, LabelData labelData) > translateLabelTexts = new List<(int assetId, int labelId, TextMeshProUGUI listText, LabelData labelData) >();
        public void AddLabelTextToTranslator((int assetId, int labelId, TextMeshProUGUI listText, LabelData labelData)  serverText)
        {
            translateLabelTexts.Add(serverText);
            TranslateLabelText(serverText);
        }
        
        private void TranslateLabelText((int assetId, int labelId, TextMeshProUGUI listText, LabelData labelData) labelText)
        {
            if(!_contentModel.HasAssetLabels(labelText.assetId)) 
                return;

            var label = _contentModel.GetAssetById(labelText.assetId).AssetDetail.AssetContentPlatform.assetLabel.ToList().Find(al => al.labelId == labelText.labelId);

            var d = label.labelLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name);

            var ll = GetLanguageOnLayer(labelText.listText.gameObject.layer);

            if (d.ContainsKey(ll))
            {
                labelText.listText.text = d[ll];
                labelText.labelData.SetText(d[ll]);
            }
            else if (d.ContainsKey(_localizationModel.FallbackCultureCode))
            {
                labelText.listText.text = d[_localizationModel.FallbackCultureCode];
                labelText.labelData.SetText(d[_localizationModel.FallbackCultureCode]);
            }
        }
        
        public void TranslateAllLabelTexts()
        {
            translateLabelTexts.RemoveAll(t => t.listText == null);

            translateLabelTexts.ForEach(TranslateLabelText);
        }

        // System Text -------------------------------------------------------

        List<(string key, TMP_Text tmpText)> translateSystemTexts = new List<(string key, TMP_Text tmpText)>();
        public void AddSystemTextToTranslator((string key, TMP_Text tmpText) systemText)
        {
            translateSystemTexts.Add(systemText);
            TranslateSystemText(systemText);
        }
        
        public void AddSystemTextToTranslator(List<(string key, TMP_Text tmpText)> systemText)
        {
            translateSystemTexts.AddRange(systemText);
            
            systemText.ForEach(t =>
            {
                TranslateSystemText(t);
            });
        }
        
        private void TranslateSystemText((string key, TMP_Text tmpText) systemText)
        {
            systemText.tmpText.text =
                GetSystemTranslations(GetLanguageOnLayer(systemText.tmpText.gameObject.layer), systemText.key);
        }
        
        public void TranslateAllSystemTexts()
        {

            translateSystemTexts.RemoveAll(t => t.tmpText == null);

            translateSystemTexts.ForEach(TranslateSystemText);
        }

        // Tooltip -------------------------------------------------------
        List<TooltipEvents> translateTooltips = new List<TooltipEvents>();
        public void AddTooltipToTranslator(TooltipEvents te)
        {
            translateTooltips.Add(te);
            TranslateTooltip(te);
        }
        
        public void AddTooltipToTranslator(List<TooltipEvents> te)
        {
            translateTooltips.AddRange(te);
            te.ForEach(TranslateTooltip);
        }
        
        public void TranslateAllTooltips()
        {
            translateTooltips.RemoveAll(t => t == null);

            translateTooltips.ForEach(TranslateTooltip);
        }
        
        private void TranslateTooltip(TooltipEvents te)
        {
            te.SetHint(GetSystemTranslations(GetLanguageOnLayer(te.gameObject.layer), te.GetKey()));
        }

        #endregion
    }
}