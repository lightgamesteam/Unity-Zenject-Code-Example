using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class ContentMultimodelViewMediator : ContentViewMediatorBase, IInitializable
    {
        [Inject] private ContentViewMediator _contentViewMediator;
        [Inject] private DiContainer _diContainer;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ICacheService _cacheService;
        [Inject] private ContentModel _contentModel;
        [Inject] private ContentViewModel _contentViewModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private LabelData.Factory labelFactory;
        [Inject] public Camera3DModelSettings cameraSettings;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;
        [Inject] private SelectableTextToSpeechMediator _selectableTextToSpeech;
        [Inject] private ContentMultimodelPartViewMediator _multimodelPart;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        [Inject] private MultiView.Factory  _multiViewFactory;

        private bool isOnGrayscale = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityGrayscale);

        private List<MultiView>  _multiView;
        public ContentViewPC contentViewPC => _contentViewModel.contentViewPC;

        private int _modelID => _contentViewModel.secondAssetID;

        private const int startRenderLayerIndex = 10;
        
        internal Dictionary<string, Toggle> _labelTogglesMainModel = new Dictionary<string, Toggle>();
        
        internal Dictionary<string, Toggle> _labelToggles = new Dictionary<string, Toggle>();
        private Dictionary<string, LabelData> _labels = new Dictionary<string, LabelData>();
        internal Dictionary<string, ObjectHighlighter> _models = new Dictionary<string, ObjectHighlighter>();

        public void Initialize()
        {
            _multiView = _contentViewMediator.MultiView;
            
            AddListeners();
        }

        private void AddListeners()
        {        
            // Change background color
            contentViewPC.multimodelRenderView.ColorPicker.ActivationChanged.AddListener(OnColorPickerActivationFirstModelChanged);
            contentViewPC.multimodelRenderView.ColorPicker.onValueChanged.AddListener(OnColorPickerFirstModelChanged);
            
            //Zoom Plus
            contentViewPC.multimodelRenderView.ZoomPlusButton.onClick.AddListener(() => contentViewPC.multimodelRenderView.SmoothOrbitCam.Zoom(0.5f));
            
            //Zoom Minus
            contentViewPC.multimodelRenderView.ZoomMinusButton.onClick.AddListener(() => contentViewPC.multimodelRenderView.SmoothOrbitCam.Zoom(-0.5f));
            
            // Multimodel Part
            contentViewPC.multimodelRenderView.MultiModelPartButton.onClick.AddListener(() => _multimodelPart.InitMultimodelPart(contentViewPC.multimodelRenderView));
            
            //Reset
            contentViewPC.multimodelRenderView.ResetButton.onClick.AddListener(() =>
            {
                CameraResetBase(contentViewPC.smoothOrbitCam, !IsAsset360Model(_contentViewModel.mainAssetID), selfTarget: IsAsset360Model(_contentViewModel.mainAssetID));

                // contentViewPC.multimodelRenderView.SmoothOrbitCam.ResetMainValues();
                // if(!IsAsset360Model(_contentViewModel.mainAssetID))
                //     contentViewPC.multimodelRenderView.SmoothOrbitCam.AutoZoomOnTarget();
                //contentViewPC.gameObject.GetAllInSceneOnLayer<RelocatorView>().ForEach(rl => rl.ResetPosition());
            });
        }
        
        private void AddListenersMultiView()
        {        
            // Change background color
            _multiView[0].MultimodelView.ColorPicker.ActivationChanged.AddListener(OnColorPickerActivationSecondModelChanged);
            _multiView[0].MultimodelView.ColorPicker.onValueChanged.AddListener(OnColorPickerSecondModelChanged);
            
            //Zoom Plus
            _multiView[0].MultimodelView.ZoomPlusButton.onClick.AddListener(() => _multiView[0].MultimodelView.SmoothOrbitCam.Zoom(0.5f));
            
            //Zoom Minus
            _multiView[0].MultimodelView.ZoomMinusButton.onClick.AddListener(() => _multiView[0].MultimodelView.SmoothOrbitCam.Zoom(-0.5f));
            
            // Multimodel Part
            _multiView[0].MultimodelView.MultiModelPartButton.onClick.AddListener(() => _multimodelPart.InitMultimodelPart(_multiView[0].MultimodelView));
            
            //Reset
            _multiView[0].MultimodelView.ResetButton.onClick.AddListener(() =>
                {
                    CameraResetBase( _multiView[0].MultimodelView.SmoothOrbitCam, !IsAsset360Model(_contentViewModel.secondAssetID), selfTarget: IsAsset360Model(_contentViewModel.secondAssetID));

                    // _multiView[0].MultimodelView.SmoothOrbitCam.ResetMainValues();
                    // if(!IsAsset360Model(_contentViewModel.secondAssetID))
                    //     _multiView[0].MultimodelView.SmoothOrbitCam.AutoZoomOnTarget();
                    // _multiView[0].gameObject.GetAllInSceneOnLayer<RelocatorView>().ForEach(rl => rl.ResetPosition());
                });
        }

        public void InitializeModule(int id)
        {
            _signal.Fire(new PopupOverlaySignal(true, "Loading Model"));
            _signal.Fire(new LoadAssetCommandSignal(id, ModelLoaded));
        }

        private void ModelLoaded(bool isLoaded, int id, GameObject model, string msg)
        {
            _signal.Fire(new PopupOverlaySignal(false));

            if (isLoaded)
            {
                AddAssetToMultiview(model);
            }
            else
            {
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"Load Asset Error /: {msg}"));
            }
        }

        private void AddAssetToMultiview(GameObject gov)
        {
            MultiView mv = _multiViewFactory.Create(startRenderLayerIndex + _multiView.Count, MultiViewType.Multimodel);
            mv.ScaleFontSize(_accessibilityModel.ModulesFontSizeScaler);
            mv.MultimodelView.CloseScreen.onClick.AddListener(CloseMode);
            mv.MultimodelView.CloseScreen.onClick.AddListener(_multimodelPart.DestroyMode);

            mv.MultimodelView.LabelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(true);

            _multiView.Add(mv);
            
            _labelTogglesMainModel.Clear();
            _labelToggles.Clear();
            _labels.Clear();
            _models.Clear();
            
            CreateLabelListMainModel();
            CreateLabelListSecondModel();
            CreateModel(gov);
            SetActiveMainUI(false);
            SetActiveMultimodelUI(false);
            StartShowModel();
            AddListenersMultiView();
            LinkModelsToToggles(_models, _labelToggles);
            
            UpdateTopRightMenuUi();
            LoadBackground(_modelID, _multiView[0].BackgroundRawImage);

            bool is360Model = IsAsset360Model(_modelID);
            CameraResetBase(_multiView[0].MultimodelView.SmoothOrbitCam, !is360Model, selfTarget: is360Model);
        }
        
        public void SetIsEnableLineRenderer(bool value, GameObject layer)
        {
            layer.GetAllInSceneOnLayer<LabelLine>().ForEach(l => l.SetLabelLineGameObjectActive(value));
        }

        private void StartShowModel()
        {
            LinkModelsToToggles(_contentViewMediator._models, _labelTogglesMainModel);
            TurnOffLabels();

            _multiView[0].MultimodelView.RenderCamera.rect = ViewportRectPresets.RightOfTwo;

            _contentViewMediator._contentView.model3DCamera.DORect(ViewportRectPresets.LeftOfTwo, 0.5f)
                .SetEase(Ease.OutQuad)
                .onComplete += () => SetActiveMultimodelUI(true);
        }

        private void TurnOffLabels()
        {
            contentViewPC.labelsListDropdownToggle.ToggleGroupContainer.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
            foreach (var labelToggle in _contentViewMediator._labelToggles)
            {
                labelToggle.Value.isOn = false;
            }

            contentViewPC.multimodelRenderView.LabelsListDropdownToggle.ToggleGroupContainer.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
            
            foreach (var labelToggle in _labelTogglesMainModel)
            {
                labelToggle.Value.isOn = false;
            }
        }

        public void CloseMode()
        {
            LinkModelsToToggles(_contentViewMediator._models, _contentViewMediator._labelToggles);
            TurnOffLabels();
            ReplicateColorPickerSettingsFromSingleToMultiMode();
            CloseAllOpenedDescriptionsOnSecondMultiView();

            SetActiveMultimodelUI(false);
            _multiView[0].MultimodelView.RenderCamera.rect = ViewportRectPresets.RightOfTwo;

            _contentViewMediator._contentView.model3DCamera.DORect(ViewportRectPresets.FullScreen, 0.5f)
                .SetEase(Ease.OutQuad)
                .onComplete += () => OnCloseMode();
        }

        private void OnCloseMode()
        {
            SetActiveMainUI(true);

            MonoBehaviour.Destroy(_multiView[0].gameObject);
            _multiView.RemoveAt(0);
            _contentViewMediator.ApplyCurrentContentViewLanguage();
        }

        private void CreateModel(GameObject gov)
        {
            GameObject instance = CreateModel(gov, "model", _multiView[0].RenderLayer.transform);
        
            foreach (string labelName in _labels.Keys)
            {
                if (instance.transform.GetChildrenByName(labelName, out GameObject modelPart))
                {
                    ObjectHighlighter oh = modelPart.gameObject.AddOneComponent<ObjectHighlighter>();
                    _models.Add(labelName, oh);
                    oh.ID = _modelID;
                    oh.SetColor(_labels[labelName].headerColor);
                }
            }
            
            instance.transform.parent.SetLayer(_multiView[0].RenderLayer.layer);

            AddIKView(instance, _modelID);
        }

        private void CreateLabelListMainModel()
        {
            if (!_contentModel.HasAssetLabels(_contentViewModel.mainAssetID))
            {
                ShowLabelsButton(contentViewPC.multimodelRenderView, false);
                return;
            }
        
            contentViewPC.multimodelRenderView.LabelsListDropdownToggle.ClearDropdownToggle();
        
            Toggle selectAll = contentViewPC.multimodelRenderView.LabelsListDropdownToggle.Template.GetComponent<Toggle>();
            ShowLabelsButton(contentViewPC.multimodelRenderView, true);

            selectAll.onValueChanged.RemoveAllListeners();
            selectAll.GetComponent<Toggle>().onValueChanged
                .AddListener(value => SetIsEnableLineRenderer(value, contentViewPC.model3DCamera.gameObject));

            foreach (var v in _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssetContentPlatform.assetLabel)
            {
                ColorUtility.TryParseHtmlString(v.highlightColor, out Color newCol);
            
                var itemKey = v.labelLocal.First(assetLocal => assetLocal.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;
                var localizedText = GetCurrentTranslationsForItem(v.labelLocal.ConvertToLocalName());

                LabelData ld =  _contentViewMediator._labels[itemKey];
                ld.IsMultiViewSecond = false;
            
                if (ld.gameObject.HasComponent(out LocalizationTextView ldLocalizationTextView))
                {
                    ldLocalizationTextView.AddLocalization(_contentViewModel.mainAssetID, v.labelLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name));
                }
            
                Toggle tgl = contentViewPC.multimodelRenderView.LabelsListDropdownToggle.CreateLabelToggle(localizedText, ld.gameObject);
                tgl.onValueChanged.AddListener(value => CheckSelectAllActive(selectAll));

                LocalizationTextView ltv = tgl.GetComponentInChildren<TextMeshProUGUI>().gameObject.AddComponent<LocalizationTextView>();
                ltv.AddLocalization(_contentViewModel.mainAssetID, v.labelLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name));
                _labelTogglesMainModel.Add(itemKey, tgl);
                
                SetLabelDescriptionButtonListener(ld, _contentViewModel.CurrentContentViewCultureCode);
            }
        }

        private void ShowLabelsButton(RenderView renderView, bool isActive)
        {
            renderView.LabelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(isActive);
            renderView.MultiModelPartButton.gameObject.SetActive(isActive);
        }

        private void UpdateTopRightMenuUi()
        {
            Dictionary<string, string> dropdownLabel = 
                new Dictionary<string, string>(_localizationModel.GetAllTranslationsByKey(LocalizationConstants.LabelsKey));
            Dictionary<string, string> dropdownLanguage = 
                new Dictionary<string, string>(_localizationModel.GetAllTranslationsByKey(LocalizationConstants.SelectLanguageKey));

            LocalizationTextView ltv;
        
            ltv = contentViewPC.multimodelRenderView.LabelsListDropdownToggle.DropdownLabel.GetComponent<LocalizationTextView>();
            ltv.AddLocalization(_contentViewModel.mainAssetID, dropdownLabel);

            ltv = contentViewPC.multimodelRenderView.LanguageListDropdownToggle.DropdownLabel.GetComponent<LocalizationTextView>();
            ltv.AddLocalization(_contentViewModel.mainAssetID, dropdownLanguage);
        
            ltv = _multiView[0].MultimodelView.LabelsListDropdownToggle.DropdownLabel.GetComponent<LocalizationTextView>();
            ltv.AddLocalization(_modelID, dropdownLabel);

            ltv = _multiView[0].MultimodelView.LanguageListDropdownToggle.DropdownLabel.GetComponent<LocalizationTextView>();
            ltv.AddLocalization(_modelID, dropdownLanguage);
        }

        public void SetLanguage(bool isThis, RenderView renderView, string cultureCode)
        {
            if(!isThis)
                return;
            
            // Translate DropdownToggle Label, 3D Label on Layer
            FindComponentExtension.GetAllInSceneOnLayer<LocalizationTextView>(SceneNameConstants.Module3DModel, renderView.GetRenderLayerIndex())
                .ForEach(ltv =>
                {
                    ltv.SetLocalizedText(cultureCode);
                    
                    if (ltv.gameObject.HasComponent(out LabelData label))
                    {
                        SetLabelDescriptionButtonState(label, cultureCode);
                        SetLabelDescriptionButtonListener(label, cultureCode);
                    }
                });
            
            // Translate Tooltip Hint
            FindComponentExtension.GetAllInSceneOnLayer<TooltipEvents>(SceneNameConstants.Module3DModel, renderView.GetRenderLayerIndex()).ForEach(t =>
            {
                if(t.GetType() != typeof(DynamicTooltipEvents))
                    t.SetHint(_localizationModel.GetSystemTranslations(cultureCode, t.GetKey()));
            });
            
            _contentViewModel.SetLanguageOnLayer(renderView.GetRenderLayerIndex(), cultureCode);
            
            //Color Piker
            var activate = _contentViewModel.GetSystemTranslations(cultureCode, LocalizationConstants.ActivateKey);
            renderView.ColorPicker.transform.GetComponentByName<TextMeshProUGUI>("ActivateColorPikerLabel").text = activate;
            
            //Translate Select all
            var translateSelectAll = _localizationModel.GetSystemTranslations(cultureCode, LocalizationConstants.SelectAllKey); //_localizationModel.AllSystemTranslations[cultureCode][LocalizationConstants.SelectAllKey];
            renderView.LabelsListDropdownToggle.Template.GetComponentInChildren<TextMeshProUGUI>().text = translateSelectAll;
            
            // Sort List in Dropdown by Alphabetical order
            renderView.LabelsListDropdownToggle.SortToggleItemByText();

            var isSecondModelChanged = IsSecondMultiView(renderView.GetRenderLayerIndex());
            
            if (isSecondModelChanged)
            {
                SetStudentDescriptionButtonState(_multiView[0].MultimodelView.StudentDescriptionButton, _contentViewModel.secondAssetID, cultureCode);
                SetStudentDescriptionButtonListener(_multiView[0].MultimodelView.StudentDescriptionButton, _contentViewModel.secondAssetID.ToString(), cultureCode, true);
            }
            else
            {
                SetStudentDescriptionButtonState(contentViewPC.multimodelRenderView.StudentDescriptionButton, _contentViewModel.mainAssetID, cultureCode);
                SetStudentDescriptionButtonListener(contentViewPC.multimodelRenderView.StudentDescriptionButton, _contentViewModel.mainAssetID.ToString(), cultureCode);
            }
            
            UpdateDescriptionLanguage(cultureCode, isSecondModelChanged);
        }

        private bool IsSecondMultiView(int currentLayerIndex)
        {
            return currentLayerIndex > Module3DModelConstants.MultiViewMainLayer;
        }

        private void CreateLabelListSecondModel()
        {
            if (!_contentModel.HasAssetLabels(_modelID))
            {
                ShowLabelsButton(_multiView[0].MultimodelView, false);
                return;
            }
            
            Toggle selectAll = _multiView[0].MultimodelView.LabelsListDropdownToggle.Template.GetComponent<Toggle>();
            
            _multiView[0].MultimodelView.LabelsListDropdownToggle.ClearDropdownToggle();
            ShowLabelsButton(_multiView[0].MultimodelView, true);
                
            selectAll.onValueChanged.RemoveAllListeners();
            selectAll.onValueChanged
                .AddListener(value => SetIsEnableLineRenderer(value, _multiView[0].RenderLayer));

            foreach (var v in _contentModel.GetAssetById(_modelID).AssetDetail.AssetContentPlatform.assetLabel)
            {
                ColorUtility.TryParseHtmlString(v.highlightColor, out Color newCol);
                
                string itemKey = v.labelLocal.First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;
                string localizedText = GetCurrentTranslationsForItem(v.labelLocal.ConvertToLocalName());

                LabelData ld = labelFactory.Create(itemKey, newCol);
                ld.ID = _modelID;
                ld.LabelId = ld.ID + "_" + v.labelId;
                ld.LabelLocalNames = v.labelLocal;
                ld.IsMultiViewSecond = true;
                ld.SetModelPartName(itemKey);
                //ld.SetLabelScale(_accessibilityModel.ModulesFontSizeScaler);

                if (ld.gameObject.HasComponent<LocalizationTextView>())
                {
                    LocalizationTextView ldLocalizationTextView = ld.gameObject.GetComponent<LocalizationTextView>();
                    ldLocalizationTextView.AddLocalization(_modelID, v.labelLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name));
                }

                ld.transform.SetParent(_multiView[0].RenderLayer.transform);
                ld.transform.SetLayer(_multiView[0].RenderLayer.layer);

                ld.transform.position = new Vector3(v.position.x, v.position.y, v.position.z);
                ld.transform.rotation = Quaternion.Euler(v.rotation.x, v.rotation.y, v.rotation.z);
                //ld.transform.localScale = new Vector3(v.Scale.X, v.Scale.Y, v.Scale.Z);

                Toggle tgl = _multiView[0].MultimodelView.LabelsListDropdownToggle.CreateLabelToggle(localizedText, ld.gameObject);
                tgl.onValueChanged.AddListener(value => CheckSelectAllActive(selectAll));
                
                LocalizationTextView ltv = tgl.GetComponentInChildren<TextMeshProUGUI>().gameObject.AddComponent<LocalizationTextView>();
                ltv.AddLocalization(_modelID, v.labelLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name));

                _labels.Add(itemKey, ld);
                _labelToggles.Add(itemKey, tgl);
                
                SetLabelDescriptionButtonListener(ld, _contentViewModel.CurrentContentViewCultureCode);
            }
        }

        private void SetActiveMainUI(bool isActive)
        {
            //UI
            // Bot Panel
            contentViewPC.bottomLeftMenu.gameObject.SetActive(isActive);
                    
            //Model Name
            contentViewPC.modelNameLabel.transform.parent.parent.gameObject.SetActive(isActive);
            
            //Main Mode Panel
            contentViewPC.rightMenuPanel.SetActive(isActive  && contentViewPC.rightMenuToggle.isOn);
            contentViewPC.topLeftMenuPanel.SetActive(isActive);
            contentViewPC.topRightMenuPanel.SetActive(isActive);

            // Label HotSpot
            LabelData.SetActiveHotSpotAction?.Invoke(isActive);
            
            if (isActive)
            {
                SetActiveMultimodelUI(false);
            }
        }

        private void SetActiveMultimodelUI(bool isActive) 
        {
            contentViewPC.multimodelRenderView.gameObject.SetActive(isActive);
            
            if (isActive)
            {
                contentViewPC.multimodelRenderView.SmoothOrbitCam.ResetMainValues();
                if(!IsAsset360Model(_contentViewModel.mainAssetID))
                    contentViewPC.multimodelRenderView.SmoothOrbitCam.AutoZoomOnTarget();

                ReplicateColorPickerSettingsFromMultiToSingleMode();
            }
            else
            {
                bool is360 = IsAsset360Model(_contentViewModel.mainAssetID);
                CameraResetBase(contentViewPC.multimodelRenderView.SmoothOrbitCam, !is360, selfTarget: is360);
            }

            if (isActive)
            {
                contentViewPC.multimodelRenderView.MultiModelPartButton.transform.parent.gameObject.SetActive(true);
                contentViewPC.multimodelRenderView.CloseScreen.gameObject.SetActive(false);
                contentViewPC.multimodelRenderView.MultiModelPartPanel.gameObject.SetActive(false);

                CreateLanguageUI(contentViewPC.multimodelRenderView);
                CreateLanguageUI(_multiView[0].MultimodelView);
            }

            var backgroundColor = contentViewPC.multimodelRenderView.RenderCamera.backgroundColor;
            contentViewPC.multimodelRenderView.ColorPicker.awakeColor = backgroundColor;
            contentViewPC.multimodelRenderView.ColorPicker.ResetColor(backgroundColor); 
        }

        void CreateLanguageUI(RenderView rv)
        {
            CreateLanguagesDropdownToggle(rv.LanguageListDropdownToggle).ForEach(item =>
            {
                item.toggle.onValueChanged.AddListener(isOn => { SetLanguage(isOn, rv, item.cultureCode); });

                if (item.cultureCode == _contentViewModel.CurrentContentViewCultureCode)
                {
                    item.toggle.SetValue(true, true);
                }
            });
        }
        
        #region Color Picker
        
        private void OnColorPickerActivationFirstModelChanged(bool isActivated)
        {
            contentViewPC.backgroundRawImage.gameObject.SetActive(!isActivated);
        }

        private void OnColorPickerFirstModelChanged(Color color)
        {
            contentViewPC.multimodelRenderView.RenderCamera.backgroundColor = color;
        }
        
        private void OnColorPickerActivationSecondModelChanged(bool isActivated)
        {
            _multiView[0].BackgroundRawImage.gameObject.SetActive(!isActivated);
        }

        private void OnColorPickerSecondModelChanged(Color color)
        {
            _multiView[0].MultimodelView.RenderCamera.backgroundColor = color;
        }

        private void ReplicateColorPickerSettingsFromSingleToMultiMode()
        {
            if (contentViewPC.colorPicker.IsInitialized)
            {
                contentViewPC.colorPicker.ActivatedToggle.isOn = contentViewPC.multimodelRenderView.ColorPicker.ActivatedToggle.isOn;
                contentViewPC.colorPicker.ResetColor(contentViewPC.multimodelRenderView.RenderCamera.backgroundColor);   
            }
        }

        private void ReplicateColorPickerSettingsFromMultiToSingleMode()
        {
            if (contentViewPC.colorPicker.IsInitialized)
            {
                contentViewPC.multimodelRenderView.ColorPicker.ActivatedToggle.isOn = contentViewPC.colorPicker.ActivatedToggle.isOn;
                contentViewPC.multimodelRenderView.ColorPicker.ResetColor(contentViewPC.model3DCamera.backgroundColor);
            }

            if (contentViewPC.multimodelRenderView.ColorPicker.gameObject.activeSelf)
            {
                contentViewPC.multimodelRenderView.ColorPicker.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}