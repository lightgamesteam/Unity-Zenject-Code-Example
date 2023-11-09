using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP;
using Module.Core;
using TDL.Commands;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using TDL.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class ContentViewMediator : ContentViewMediatorBase, IInitializable, IDisposable, IMediator
    {
        [Inject] private ContentMultimodelViewMediator _contentMultimodelViewMediator;
        [Inject] private FeedbackModel _feedbackModel;
        [Inject] private SelectableTextToSpeechMediator _selectableTextToSpeech;
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        private bool isOnGrayscale = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityGrayscale);

        public ContentViewPC _contentView => _contentViewModel.contentViewPC;
        
        //public int _assetID;
        public int _layerIndex => _contentView.gameObject.layer;

        internal Dictionary<string, Toggle> _labelToggles = new Dictionary<string, Toggle>();
        internal Dictionary<string, LabelData> _labels = new Dictionary<string, LabelData>();
        internal Dictionary<string, ObjectHighlighter> _models = new Dictionary<string, ObjectHighlighter>();

        private Dictionary<int, TextMeshProUGUI> _associatedContentsTexts = new Dictionary<int, TextMeshProUGUI>();

        private Action<bool, int, HTTPRequestStates, float> _onAssetDetailsDownloadedMethodToCall;

        #region Initialize

        public void Initialize()
        {
            _contentViewModel.UpdateCurrentLanguage();

            SubscribeOnListeners();
            _contentViewModel.contentViewPC = (ContentViewPC)_contentViewFactory.Create();
            _contentViewModel.ContentMain = _contentView.ContentMainCanvasGroup.transform;
        
            _contentView.GetReferenceUI();
            _contentView.SubscribeOnListeners();
            SetActiveView(false);

            ChangeFontSize();
            SetFeedbackAvailability();
            SetStudentNoteVisibility(_contentView);

            InitializeContents();
            ConnectListeners();
        }

        public void SetActiveView(bool isActive)
        {
            _contentView.gameObject.SetActive(isActive);
        }

        public void OnViewEnable()
        {
            _signal.Subscribe<SendFeedbackViewSignal>(SendFeedback);
            _signal.Subscribe<FeedbackSentOkClickViewSignal>(OnFeedbackSentOkClick);
            _signal.Subscribe<CancelFeedbackViewSignal>(CancelFeedback);
            _signal.Subscribe<ShowMainFeedbackPanelViewSignal>(OnShowMainFeedbackPanel);
            _signal.Subscribe<OnDescriptionCloseClickViewSignal>(OnDescriptionClose);
            _signal.Subscribe<LoadAndPlayDescriptionViewSignal>(LoadAndPlayDescription);
            _signal.Subscribe<PauseAllExceptActiveDescriptionViewSignal>(PauseAllExceptActiveDescription);
            _signal.Subscribe<OnDescriptionBlockModelMovementsViewSignal>(OnDescriptionBlockModelMovements);
            
            _contentView.multimodelSearchButton.SearchInputField.onValueChanged.AddListener(OnSearchInputChanged);
        }

        public void OnViewDisable()
        {
            _signal.Unsubscribe<SendFeedbackViewSignal>(SendFeedback);
            _signal.Unsubscribe<FeedbackSentOkClickViewSignal>(OnFeedbackSentOkClick);
            _signal.Unsubscribe<CancelFeedbackViewSignal>(CancelFeedback);
            _signal.TryUnsubscribe<ShowMainFeedbackPanelViewSignal>(OnShowMainFeedbackPanel);
            _signal.Unsubscribe<OnDescriptionCloseClickViewSignal>(OnDescriptionClose);
            _signal.Unsubscribe<LoadAndPlayDescriptionViewSignal>(LoadAndPlayDescription);
            _signal.Unsubscribe<PauseAllExceptActiveDescriptionViewSignal>(PauseAllExceptActiveDescription);
            _signal.Unsubscribe<OnDescriptionBlockModelMovementsViewSignal>(OnDescriptionBlockModelMovements);
            
            _contentView.multimodelSearchButton.SearchInputField.onValueChanged.RemoveAllListeners();
        }

        private void ChangeFontSize()
        {
            if(_accessibilityModel.ModulesFontSizeScaler <= 0 || _accessibilityModel.ModulesFontSizeScaler == 1f)
                return;
            
            _contentView.gameObject.GetAllInScene<TextMeshProUGUI>(true).ForEach(t => t.fontSize /= _accessibilityModel.ModulesFontSizeScaler);
            _contentView.gameObject.GetAllInScene<Text>().ForEach(t => t.fontSize = (int) (t.fontSize / _accessibilityModel.ModulesFontSizeScaler));
        }
        
        private void SubscribeOnListeners()
        {
            _feedbackModel.OnShowMainFeedback += ShowMainFeedback;
            _feedbackModel.OnShowSentFeedback += ShowSentFeedback;

            _signal.Subscribe<SubscribeOnContentViewSignal>(OnViewEnable);
            _signal.Subscribe<UnsubscribeFromContentViewSignal>(OnViewDisable);
            _signal.Subscribe<VideoRecordingStateSignal>(OnChangeRecordingState);
        }

        public void InitializeModule(GameObject go, int id = -1)
        {
            if (id < 0)
                _contentViewModel.mainAssetID = _contentModel.SelectedAsset.Asset.Id;
            else
                _contentViewModel.mainAssetID = id;

            InitializeModuleAction?.Invoke(_contentViewModel.mainAssetID);
            //SetActiveView(true);
            CreateLabelList();
            CreateModelPrefab(go);
            LinkModelsToToggles(_models, _labelToggles);
            CreateLanguagesDropdownToggle(_contentView.languagesDropdownToggle).ForEach(item =>
            {
                item.toggle.onValueChanged.AddListener(isOn => { ChangeLanguageByToggle(isOn, item.cultureCode); });

                if (item.cultureCode == _contentViewModel.CurrentContentViewCultureCode)
                    item.toggle.SetValue(true, true);
            });

            CreateAssociatedContentsDropdownToggle();
            SetupModelPart();
            //SelectLanguageOnInitializeModule();
            ShowReturnButton();
            ShowPlayAnimationButton(go);
            LoadBackground(_contentViewModel.mainAssetID, _contentView.backgroundRawImage);
            InitializePaint();

            CameraResetBase(_contentView.smoothOrbitCam, selfTarget: IsAsset360Model(_contentViewModel.mainAssetID));
        }

        private void ShowReturnButton()
        {
            if (_content3DModelHistory.ContainItems())
            {
                _contentView.returnButton.gameObject.SetActive(true);
                _contentView.backButton.gameObject.SetActive(false);
                
                var index = _content3DModelHistory.GetLastItem();
                _contentView.returnButton.onClick.RemoveAllListeners();
                _contentView.returnButton.onClick.AddListener(() => OpenNewAsset(index));
                _contentView.returnButton.onClick.AddListener(() => _content3DModelHistory.UndoHistory());
            }
            else
            {
                _contentView.returnButton.gameObject.SetActive(false);
                _contentView.backButton.gameObject.SetActive(true);
            }

            _content3DModelHistory.AddHistory(_contentViewModel.mainAssetID);
        }
        
        public void Dispose()
        {
            DisconnectListeners();
            if (_feedbackModel != null)
            {
                _feedbackModel.OnShowMainFeedback -= ShowMainFeedback;
                _feedbackModel.OnShowSentFeedback -= ShowSentFeedback;
            }
        
            _signal.Unsubscribe<SubscribeOnContentViewSignal>(OnViewEnable);
            _signal.Unsubscribe<UnsubscribeFromContentViewSignal>(OnViewDisable);
            _signal.Unsubscribe<VideoRecordingStateSignal>(OnChangeRecordingState);

            if(_userLoginModel.IsLoggedAsUser)
                _signal.Fire<ShowHomeScreenCommandSignal>();
            else
            {
                _signal.Fire<ShowLoginScreenCommandSignal>();
            }
        }

        #endregion

        #region CreateView

        private void CreateLabelList()
        {
            if (!_contentModel.HasAssetLabels(_contentViewModel.mainAssetID))
            {
                _contentView.labelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(false);
                return;
            }

            Toggle selectAll = _contentView.labelsListDropdownToggle.Template.GetComponent<Toggle>();
        
            _contentView.labelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(true);
            selectAll.onValueChanged.RemoveAllListeners();

            foreach (var v in _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssetContentPlatform.assetLabel)
            {
                ColorUtility.TryParseHtmlString(v.highlightColor, out Color newCol);
            
                var itemKey = v.labelLocal
                    .First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;

                var ld = labelFactory.Create(itemKey, newCol);
                ld.ID = _contentViewModel.mainAssetID;
                ld.LabelId = ld.ID + "_" + v.labelId;
                ld.LabelLocalNames = v.labelLocal;
                ld.SetModelPartName(itemKey);

                if (v.labelHotSpot != null)
                {
                    if (v.labelHotSpot.Length > 0)
                    {
                        ld.SetHotSpotButtonActive(true);
                        ld.GetHotSpotButton().onClick.AddListener(() => ShowHotSpotListPanel(v));
                    }
                    else
                    {
                        ld.SetHotSpotButtonActive(false);
                    }
                }
                else
                {
                    ld.SetHotSpotButtonActive(false);
                }
                
                SetLabelDescriptionButtonListener(ld, _contentViewModel.CurrentContentViewCultureCode);

                //ld.SetLabelScale(_accessibilityModel.ModulesFontSizeScaler);
                ld.transform.position = new Vector3(v.position.x, v.position.y, v.position.z);
                ld.transform.rotation = Quaternion.Euler(v.rotation.x, v.rotation.y, v.rotation.z);
                //ld.transform.localScale = new Vector3(v.Scale.X, v.Scale.Y, v.Scale.Z);

                var tgl = _contentView.labelsListDropdownToggle.CreateLabelToggle(itemKey, ld.gameObject);
            
                tgl.onValueChanged.AddListener(value => ld.GetComponent<LabelLine>().SetLabelLineGameObjectActive(value));
            
                tgl.onValueChanged.AddListener(value => CheckSelectAllActive(selectAll));
            
                _labels.Add(itemKey, ld);
                _labelToggles.Add(itemKey, tgl);
            }
        }

        private void ShowHotSpotListPanel(Assetlabel assetlabel)
        {
            List<(string label, int assetId, string assetType, bool hasPlusButton)> hotSpotList = 
                new List<(string label, int assetId, string assetType, bool hasPlusButton)>();

            foreach (var associatedAsset in assetlabel.labelHotSpot)
            {
                string labelName = GetAssociatedContentTranslation(associatedAsset.AssetId, _contentViewModel.CurrentContentViewCultureCode);
                string type = associatedAsset.Type.ToLower();
                bool hasPlus = !type.Equals(AssetTypeConstants.Type_2D_Video) &&
                               !type.Equals(AssetTypeConstants.Type_3D_Video);

                hotSpotList.Add((labelName, associatedAsset.AssetId, type, hasPlus));
            }

            string panelName = GetAssociatedContentTranslation(_contentViewModel.mainAssetID, _contentViewModel.CurrentContentViewCultureCode)
                               + ": " + GetCurrentTranslationsForItem(assetlabel.labelLocal.ConvertToLocalName());

            _signal.Fire(new PopupHotSpotListViewSignal(panelName, hotSpotList, _contentViewModel.CurrentContentViewCultureCode, LabelHotSpotCallback));
        }

        private void LabelHotSpotCallback(int id, string assetType, bool isPlusButton)
        {
            if (assetType.Equals(AssetTypeConstants.Type_2D_Video) || assetType.Equals(AssetTypeConstants.Type_3D_Video))
            {
                OnAssociatedContentClick(id);
            }
            else
            {
                if(isPlusButton)
                    StartMultimodelMode(id);
                else
                    OpenNewAsset(id);
            }
        }

        private void CreateAssociatedContentsDropdownToggle()
        {
            var associatedContent = _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssociatedContents;

            if (associatedContent == null || associatedContent.Length == 0)
            {
                _contentView.relatedContentsDropdownToggle.DropdownListToggle.gameObject.SetActive(false);
                return;
            }
        
            _contentView.relatedContentsDropdownToggle.DropdownListToggle.gameObject.SetActive(true);
        
            foreach (var ac in associatedContent)
            {
                var data = _contentModel.GetAssetById(ac.AssetId);

                if (data != null)
                {
                    var contentName = GetAssociatedContentTranslation(data.LocalizedName, _localizationModel.CurrentLanguageCultureCode);
                    
                    var btn = _contentView.relatedContentsDropdownToggle.CreateAssociatedContentButton(contentName);

                    if (_associatedContentsTexts.ContainsKey(data.Asset.Id))
                    {
                        _signal.Fire(new SendCrashAnalyticsCommandSignal("Already added associated content: " + ac.Name + ", assetID: " + _contentViewModel.mainAssetID));
                    }
                    else
                    {
                        _associatedContentsTexts.Add(data.Asset.Id, btn.gameObject.GetFirstInChildren<TextMeshProUGUI>());
                    }

                    if (ac.Type.ToLower().Equals(AssetTypeConstants.Type_2D_Video)
                        || ac.Type.ToLower().Equals(AssetTypeConstants.Type_3D_Video))
                    {
                        btn.onClick.AddListener(() => OnAssociatedContentClick(data.Asset.Id));
                    }
                    else
                    {
                        btn.onClick.AddListener(() =>
                        {
                            OpenNewAsset(data.Asset.Id);
                        });

                        if (ac.Type.ToLower().Equals(AssetTypeConstants.Type_3D_Model))
                        {
                            Button plusBtn = btn.transform.parent.Get<Button>("X_Button_Plus");
                            plusBtn.gameObject.SetActive(true);
                            plusBtn.onClick.AddListener(() => StartMultimodelMode(data.Asset.Id));
                        }
                    }
                }
            }

            if (_associatedContentsTexts.Count == 0)
            {
                _contentView.relatedContentsDropdownToggle.DropdownListToggle.gameObject.SetActive(false);
            }
        }

        internal void OnAssociatedContentClick(int Id)
        {
            _contentModel.AssetDetailsSignalSource = new AssetItemClickCommandSignal(Id, -1);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {Id}));
        }

        internal void OpenNewAsset(int Id)
        {
            _signal.Fire(new ShowDebugLogCommandSignal($"@@@ ContentViewMediator : Open New Asset > ID = {Id} | Name = {_contentModel?.GetAssetById(Id)?.Asset?.Name}"));

            SetupPopupProgressReplica(Id, _contentView.popupProgressFactory);

            _contentModel.AssetDetailsSignalSource = new DownloadAssetCommandSignal(Id, OpenNewAsset);
            _signal.Fire(new Module3dStartAssetDetailsCommandSignal(Id));
        }

        private void OpenNewAsset(bool isDownloaded, int id, HTTPRequestStates downloadState, float downloadProgress)
        {
            Debug.Log(downloadProgress);
            if (isDownloaded)
            {
                if (_contentView.popupProgressFactory.Replica(id).IsActive())
                {
                    _signal.Fire(new LoadAssetCommandSignal(id, ModelLoaded));
                    _signal.Fire(new PopupOverlaySignal(true, _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
                }

                _contentView.popupProgressFactory.Replica(id).SelfDestroy();
            }
            else
            {
                switch (downloadState)
                {
                    case HTTPRequestStates.Processing:
                        if (_contentView.popupProgressFactory.Replica(id, out PopupProgress pp)) ;
                        {
                            pp.PopupProgressLabel.SetText($"{(int) downloadProgress} %");
                            pp.PopupSlider.value = (int) downloadProgress;
                        }

                        break;

                    case HTTPRequestStates.Error:
                        _contentView.popupProgressFactory.Replica(id).SelfDestroy();
                        _signal.Fire(new SendCrashAnalyticsCommandSignal("@@@ Load Asset Error /: Error"));

                        break;

                    case HTTPRequestStates.ConnectionTimedOut:
                        _contentView.popupProgressFactory.Replica(id).SelfDestroy();
                        _signal.Fire(new SendCrashAnalyticsCommandSignal("@@@ Download Asset Error /: Connection TimedOut"));

                        break;
                }
            }
        }

        private void ModelLoaded(bool isLoaded, int id, GameObject model, string msg)
        {
            if (isLoaded)
            {
                _contentView.labelsListDropdownToggle.ClearDropdownToggle();
                _contentView.languagesDropdownToggle.ClearDropdownToggle();
                _contentView.relatedContentsDropdownToggle.ClearDropdownToggle();

                _contentView.ClearView(load3DModelContainer.transform);

                _labelToggles.Clear();
                _labels.Clear();
                _models.Clear();
                _associatedContentsTexts.Clear();

                _signal.Fire(new PopupOverlaySignal(false));
                InitializeModule(model, id);
            }
            else
            {
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"Load Asset Error /: {msg}"));
            }
        }

        private void CreateModelPrefab(GameObject prefab)
        {
            GameObject instance = CreateModel(prefab, "model", load3DModelContainer.transform);
        
            foreach (string labelName in _labels.Keys)
            {
                if (instance.transform.GetChildrenByName(labelName, out GameObject modelPart))
                {
                    ObjectHighlighter oh = modelPart.gameObject.AddOneComponent<ObjectHighlighter>();
                    _models.Add(labelName, oh);
                    oh.ID = _contentViewModel.mainAssetID;
                    oh.SetColor(_labels[labelName].headerColor);
                }
            }
            
            load3DModelContainer.transform.SetLayer(_layerIndex);

            AddIKView(instance, _contentViewModel.mainAssetID);
        }

        private void ShowPlayAnimationButton(GameObject model)
        {
            if (model.HasComponent<Animator>() && model.GetComponent<Animator>().runtimeAnimatorController != null)
            {
                _contentView.pauseAnimationButton.gameObject.SetActive(true);
                _contentView.playAnimationButton.gameObject.SetActive(false);
            }
            else
            {
                _contentView.pauseAnimationButton.gameObject.SetActive(false);
                _contentView.playAnimationButton.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Language

        public void ApplyCurrentContentViewLanguage()
        {
            string lang = _localizationModel.GetLanguageByCultureCode(_contentViewModel.CurrentContentViewCultureCode);

            _contentView.languagesDropdownToggle.ToggleGroupContainer.transform
                .GetComponentsInChildren<TextMeshProUGUI>().ToList().ForEach(tmp =>
                {
                    if (tmp.text == lang)
                    {
                        Toggle t = tmp.transform.parent.GetComponent<Toggle>();
                        t.isOn = true;
                        t.onValueChanged.Invoke(true);
                    }
                });
        }

        private void ChangeLanguageByToggle(bool isThis, string cultureCode)
        {
            if (isThis)
            {
                _contentViewModel.SetLanguageOnLayer(_layerIndex, cultureCode, true);

                if (_contentModel.GetAssetById(_contentViewModel.mainAssetID).LocalizedName.Count > 1)
                    _contentView.SetLabel(GetCurrentTranslationsForItem(_contentModel.GetAssetById(_contentViewModel.mainAssetID).LocalizedName));
                else
                    _contentView.SetLabel(_contentModel.GetAssetById(_contentViewModel.mainAssetID).LocalizedName[_localizationModel.FallbackCultureCode]);

                UpdateTopRightMenuUi();
                UpdateFeedbackMenuUi();
                UpdateDescriptionLanguage(cultureCode);
                SetLanguageTextInModel();
                
                //Color Piker
                var activate = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.ActivateKey);
                _contentView.colorPicker.transform.GetComponentByName<TextMeshProUGUI>("ActivateColorPikerLabel").text = activate;
                
                _contentView.gameObject.GetAllInSceneOnLayer<TooltipEvents>().ForEach(t =>
                {
                    var keyTranslation = t.GetKey();
                    if (!string.IsNullOrEmpty(keyTranslation))
                    {
                        t.SetHint(_contentViewModel.GetCurrentSystemTranslations(keyTranslation));
                    }
                });
                
                _contentView.gameObject.GetAllInSceneOnLayer<ToggleTooltipEvents>().ForEach(t =>
                {
                    var key = t.GetTrueKey();
                    if (!string.IsNullOrEmpty(key))
                    {
                        t.SetTrueHint(_contentViewModel.GetCurrentSystemTranslations(key));
                        key = string.Empty;
                    }
                    
                    key = t.GetFalseKey();
                    if (!string.IsNullOrEmpty(key))
                    {
                        t.SetFalseHint(_contentViewModel.GetCurrentSystemTranslations(key));
                    }
                });

                if (_contentModel.HasAssetLabels(_contentViewModel.mainAssetID))
                {
                    _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssetContentPlatform.assetLabel.ToList().ForEach(item =>
                    {
                        var itemKey = item.labelLocal.First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;
                        var translate = GetCurrentTranslationsForItem(_contentViewModel.mainAssetID, itemKey);

                        var labelItem = _labelToggles[itemKey].GetComponentInChildren<TextMeshProUGUI>();
                        labelItem.text = translate;

                        var label = _labels[itemKey];
                        label.SetText(translate);

                        SetLabelDescriptionButtonState(label, cultureCode);
                        SetLabelDescriptionButtonListener(label, cultureCode);
                    });
                    
                    _contentView.labelsListDropdownToggle.SortToggleItemByText();
                }

                //Translate Select all
                var translateSelectAll = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.SelectAllKey);
                _contentView.LabelsListDropdownSelectAllText.text = translateSelectAll;
                
                SetStudentDescriptionButtonState(_contentView.DescriptionButton, _contentViewModel.mainAssetID, cultureCode);
                SetStudentDescriptionButtonListener(_contentView.DescriptionButton, _contentViewModel.mainAssetID.ToString(), cultureCode);
            }
        }

        private void UpdateTopRightMenuUi()
        {
            var rc = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.RelatedContentKey);
            _contentView.relatedContentsDropdownToggle.DropdownLabel.text = rc;

            var relatedDropdown =
                _contentView.relatedContentsDropdownToggle.gameObject.GetFirstInChildren<TextMeshProUGUI>(true);
            if (relatedDropdown != null)
            {
                relatedDropdown.text = rc;
            }

            foreach (var associatedContentsText in _associatedContentsTexts)
            {
                associatedContentsText.Value.text =
                    GetAssociatedContentTranslation(associatedContentsText.Key, GetLanguageOnLayer(_layerIndex));
            }

            var sl = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.SelectLanguageKey);
            _contentView.languagesDropdownToggle.DropdownLabel.text = sl;

            var languageDropdown =
                _contentView.languagesDropdownToggle.gameObject.GetFirstInChildren<TextMeshProUGUI>(true);
            if (languageDropdown != null)
            {
                languageDropdown.text = sl;
            }

            var ll = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.LabelsKey);
            _contentView.labelsListDropdownToggle.DropdownLabel.text = ll;

            var labelListDropdown =
                _contentView.labelsListDropdownToggle.gameObject.GetFirstInChildren<TextMeshProUGUI>(true);
            if (labelListDropdown != null)
            {
                labelListDropdown.text = ll;
            }

            _contentView.multimodelSearchButton.SearchInputField.placeholder.GetComponent<Text>().text =
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.PlaceholderSearchForModelKey);
            
            _contentView.NoSearchResultsFound.text = 
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.NoSearchResultsKey);
        }

        private void SetLanguageTextInModel()
        {
            var go = load3DModelContainer.transform.Find("model").transform;
            if (go == null)
            {
                return;
            }

            var text_container = go.Find("Texts");
            if (text_container != null)
            {
                var language_EN = text_container.Find("en-US");
                var language_NO = text_container.Find("nb-NO");

                if (language_EN != null)
                {
                    if (language_EN.name.Equals(_contentViewModel.CurrentContentViewCultureCode))
                    {
                        language_EN.gameObject.SetActive(true);
                    }
                    else
                    {
                        language_EN.gameObject.SetActive(false);
                    }
                }

                if (language_NO != null)
                {
                    if (language_NO.name.Equals(_contentViewModel.CurrentContentViewCultureCode))
                    {
                        language_NO.gameObject.SetActive(true);
                    }
                    else
                    {
                        language_NO.gameObject.SetActive(false);
                    }
                }
            }
        }

        #endregion

        #region MultiModel
        
        private void OnSearchInputChanged(string searchValue)
        {
            var canSearch = _contentView.multimodelSearchButton.SearchInputField.text.Length >= SearchConstants.MinimumSearchSymbols;
            _contentView.SearchGoButton.interactable = canSearch;
            if (canSearch)
            {
                MultimodelSearch();
            }
        }
        
        private void OnSearchEnterClick(string searchValue)
        {
            if (IsSearchAllowed())
            {
                var isEnterPressed = Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return);
                if (isEnterPressed)
                {
                    MultimodelSearch();
                }   
            }
        }
        
        private bool IsSearchAllowed()
        {
            return _contentView.SearchGoButton.interactable;
        }

        private void MultimodelSearch()
        {
            var search = _contentView.multimodelSearchButton.SearchInputField.text;
            _signal.Fire(new Module3dGetSearchAssetsCommandSignal(search, _contentViewModel.CurrentContentViewCultureCode, new []
            {
                AssetTypeConstants.Type_3D_Model, 
                AssetTypeConstants.Type_360_Model, 
                AssetTypeConstants.Type_Rigged_Model
            }, ResultSearch));

            void ResultSearch(List<ClientAssetModel> result)
            {
                _contentView.multimodelSearchButton.ClearDropdownToggle();
                _contentViewModel.ClearCachedTextures();

                if (result == null || result.Count < 1)
                {
                    ShowNoSearchResults(true);
                    return;
                }
                
                ShowNoSearchResults(false);

                foreach (var clientAssetModel in result)
                {
                    var localizedName = GetClientAssetModelTranslation(clientAssetModel);

                    Sprite sprite = _contentViewModel.GetSpriteType(clientAssetModel.Asset.Type);
                    
                    Button btn = _contentView.multimodelSearchButton.CreateTemplate(localizedName, sprite)
                        .GetComponentInChildren<Button>();
                    
                    btn.onClick.AddListener(ActionToHideSearchModel);
                    btn.onClick.AddListener(() => _contentView.multimodelSearchButton.SearchInputField.text = "");
                    btn.onClick.AddListener(() => StartMultimodelMode(clientAssetModel.Asset.Id));

                    _signal.Fire(new LoadThumbnailsCommandSignal(clientAssetModel.Asset, btn.targetGraphic as Image));

                    if (DeviceInfo.IsPCInterface())
                    {
                        SetTooltip(btn, localizedName);
                    }
                }
            }
        }

        private void SetTooltip(Button buttonView, string localizedName)
        {
            var tooltip = buttonView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(localizedName);
        }

        public void ClosePaintMode()
        {
            if (_contentView.paintView.paint3DToggle.isOn)
                _contentView.paintView.paint3DToggle.isOn = false;
        }

        public void StartMultimodelMode(int assetId)
        {
            _signal.Fire(new ShowDebugLogCommandSignal($"@@@ ContentViewMediator : StartMultimodelMode > ID = {assetId}"));
            ClosePaintMode();
            SetupPopupProgressReplica(assetId, _contentView.popupProgressFactory);

            _contentModel.AssetDetailsSignalSource = new DownloadAssetCommandSignal(assetId, MultimodelDownloadAsset);
            _signal.Fire(new Module3dStartAssetDetailsCommandSignal(assetId));
        }

        private void MultimodelDownloadAsset(bool isDownloaded, int id, HTTPRequestStates downloadState, float downloadProgress)
        {
            if (isDownloaded)
            {
                if (_contentView.popupProgressFactory.Replica(id).IsActive())
                {
                    _contentViewModel.secondAssetID = id;

                    if (_isOnArMode)
                    {
                        _augmentedRealityMediator.AddSecondModel(id);
                    }
                    else
                    {
                        _contentMultimodelViewMediator.InitializeModule(id);
                    }
                }

                _contentView.popupProgressFactory.Replica(id).SelfDestroy();
            }
            else
            {
                switch (downloadState)
                {
                    case HTTPRequestStates.Processing:

                        if (_contentView.popupProgressFactory.Replica(id, out PopupProgress pp))
                        {
                            pp.PopupProgressLabel.SetText($"{(int) downloadProgress} %");
                            pp.PopupSlider.value = (int) downloadProgress;
                        }

                        break;

                    case HTTPRequestStates.Error:
                        _contentView.popupProgressFactory.Replica(id).SelfDestroy();

                        _signal.Fire(new SendCrashAnalyticsCommandSignal("@@@ Load Asset Error /: Error"));

                        break;

                    case HTTPRequestStates.ConnectionTimedOut:
                        _contentView.popupProgressFactory.Replica(id).SelfDestroy();

                        _signal.Fire(new SendCrashAnalyticsCommandSignal("@@@ Download Asset Error /: Connection TimedOut"));

                        break;
                }
            }
        }

        #endregion

        #region MultiPart

        public Action<string, bool> modelPartSelectedAction;
        private void SetupModelPart()
        {
            _signal.Fire(new ShowDebugLogCommandSignal($"@@@ ContentViewMediator : SetupModelPart > ID = {_contentViewModel.mainAssetID} | HasAssetLabels = {_contentModel.HasAssetLabels(_contentViewModel.mainAssetID)}"));
        
            if (!_contentModel.HasAssetLabels(_contentViewModel.mainAssetID))
            {
                _contentView.multipartButton.gameObject.SetActive(false);
                return;
            }
        
            _contentView.multipartButton.gameObject.SetActive(true);

            foreach (var item in _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssetContentPlatform.assetLabel)
            {
                var itemKey = item.labelLocal.First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;

                if (_models.ContainsKey(itemKey))
                {
                    _labelToggles[itemKey].onValueChanged.AddListener((value) => ModelPartSelected(itemKey, value));
                }
                else
                {
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("Label not found: " + itemKey + ", assetID: " + _contentViewModel.mainAssetID));
                }
            }
        }

        public void ModelPartSelected(string modelPartName, bool isSelected)
        {
            modelPartSelectedAction?.Invoke(modelPartName, isSelected);
        }

        #endregion

        #region FeedbackAndNote
        private void SetFeedbackAvailability()
        {
            _contentView.SetFeedbackAvailability(_userLoginModel.IsTeacher);
        }
        
        private void OnShowMainFeedbackPanel(ShowMainFeedbackPanelViewSignal signal)
        {
            _contentModel.AssetDetailsSignalSource = new ShowMainFeedbackPanelCommandSignal(_contentViewModel.mainAssetID, FeedbackModel.FeedbackType.Module);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {_contentViewModel.mainAssetID}));
        }

        private void ShowMainFeedback(bool status)
        {
            if (_feedbackModel.Type == FeedbackModel.FeedbackType.Module)
            {
                _contentView.ShowMainFeedbackPanel(status);

                if (!status)
                {
                    CloseFeedbackPopup();
                }
            }
        }

        private void ShowSentFeedback(bool status)
        {
            if (_feedbackModel.Type == FeedbackModel.FeedbackType.Module)
            {
                if (status)
                {
                    _contentView.FeedbackSentOkText.text =
                        _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackSentOkKey);
                }

                _contentView.ShowSentFeedbackPanel(status);
            }
        }

        private void SendFeedback(SendFeedbackViewSignal signal)
        {
            _signal.Fire(new SendFeedbackCommandSignal(_contentModel.SelectedAsset.Asset.Id, signal.FeedbackMessage));
        }

        private void CancelFeedback(CancelFeedbackViewSignal signal)
        {
            CloseFeedbackPopup();
        }

        private void OnFeedbackSentOkClick(FeedbackSentOkClickViewSignal signal)
        {
            CloseFeedbackPopup();
        }

        private void CloseFeedbackPopup()
        {
            _signal.Fire<HideFeedbackPopupCommandSignal>();
            _contentView.CloseFeedbackWindow();
        }

        private void UpdateFeedbackMenuUi()
        {
            _contentView.FeedbackTitle.text =
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.SendFeedbackTitleKey);
            _contentView.FeedbackSendText.text =
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackButtonSendKey);
            _contentView.FeedbackCancelText.text =
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackButtonCancelKey);
            _contentView.FeedbackPlaceholderText.text =
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackPlaceholderKey);
            _contentView.FeedbackSentOkText.text =
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackSentOkKey);
        }

        #endregion

        #region NewFeatures

        private void InitializeContents() 
        {
            ActionToHideSearchModel();
        }

        private void InitializePaint()
        {
            _contentView.paintView.paintViewType = PaintViewType.ContentView;
            _contentView.paintView.smoothOrbitCam = _contentView.smoothOrbitCam;
            _contentView.paintView.camera = _contentView.model3DCamera;
            _contentView.paintView.model = load3DModelContainer.transform.Find("model").gameObject;

            _asyncProcessor.Wait(0.1f, () =>
            {
                _signal.Fire(new InitializePaintSignal(_contentView.paintView));
            });
        }
        
        private void ConnectListeners() 
        {
            _contentView.backButton.onClick.AddListener(() => DisposeModule());

            _contentView.screenshotButton.onClick.AddListener(TakeScreenshot);

            _contentView.helpVideoRecordingButton.onClick.AddListener(OpenHelpVideoRecordingURL);

            _contentView.multimodelSearchButton.ToggleSearchButton.onClick.AddListener(ActionToShowSearchModel);
            _contentView.multimodelSearchButton.ToggleSearchButton.onClick.AddListener(ActionToSelectSearchInputField);

            _contentView.multimodelSearchButton.SearchCloseButton.onClick.AddListener(ActionToHideSearchModel);
            _contentView.multimodelSearchButton.SearchCloseButton.onClick.AddListener(ActionToClearSearch);
            
            _contentView.multimodelSearchButton.SearchInputField.onEndEdit.AddListener(OnSearchEnterClick);
            _contentView.SearchGoButton.onClick.AddListener(MultimodelSearch);
            
            // Reset camera position, zoom and rotation
            _contentView.resetButton.onClick.AddListener(() => CameraResetBase(_contentView.smoothOrbitCam, selfTarget: IsAsset360Model(_contentViewModel.mainAssetID)));
            
            _contentView.recorderToggle.onValueChanged.AddListener(OnRecorder);
        }

        private void DisconnectListeners() 
        {
            _contentView.screenshotButton.onClick.RemoveListener(TakeScreenshot);

            _contentView.helpVideoRecordingButton.onClick.RemoveListener(OpenHelpVideoRecordingURL);

            _contentView.multimodelSearchButton.ToggleSearchButton.onClick.RemoveListener(ActionToShowSearchModel);
            _contentView.multimodelSearchButton.ToggleSearchButton.onClick.RemoveListener(ActionToSelectSearchInputField);

            _contentView.multimodelSearchButton.SearchCloseButton.onClick.RemoveListener(ActionToHideSearchModel);
            _contentView.multimodelSearchButton.SearchCloseButton.onClick.RemoveListener(ActionToClearSearch);
            
            _contentView.multimodelSearchButton.SearchInputField.onEndEdit.RemoveAllListeners();
            _contentView.SearchGoButton.onClick.RemoveAllListeners();
        }
        
        private void OpenHelpVideoRecordingURL()
        {
            _signal.Fire(new OpenUrlCommandSignal(ServerConstants.HelpVideoRecordingUrl));
        }

    //    private bool _isOnMultipartButton;
    //    private bool _isOnMultimodelSearchToggle;
    //    private bool _isOnRealatedContentToggle;

        private void OnRecorder(bool isRecording) 
        {
            _signal.Fire(new StartVideoRecordingSignal(isRecording, _contentViewModel.mainAssetID, _contentView.recorderToggle, _contentViewModel.CurrentContentViewCultureCode));
        }
        
        private void TakeScreenshot() 
        {
            _signal.Fire(new TakeScreenshotSignal(_contentViewModel.mainAssetID, _contentViewModel.CurrentContentViewCultureCode));
        }
    
        private void OnChangeRecordingState(VideoRecordingStateSignal signal)
        {
            if (signal.State == RecordingState.StartRecording)
            {
                SetBackButtonInteractable(false);
                SetScreenshotButtonInteractable(false);
                
    //            _isOnMultipartButton = _contentView.multipartButton.gameObject.activeSelf;
    //            _isOnMultimodelSearchToggle = _contentView.multimodelSearchToggle.gameObject.activeSelf;
    //            _isOnRealatedContentToggle = _contentView.relatedContentsDropdownToggle.gameObject.activeSelf;
    //            
    //            _contentView.multipartButton.gameObject.SetActive(false);
    //            _contentView.multimodelSearchToggle.gameObject.SetActive(false);
    //            _contentView.relatedContentsDropdownToggle.gameObject.SetActive(false);
            }
            else
            {
                _asyncProcessor.Wait(0, () =>
                {
                    SetBackButtonInteractable(true);
                    SetScreenshotButtonInteractable(true);
                    _contentView.recorderToggle.isOn = false;

    //                _contentView.multipartButton.gameObject.SetActive(_isOnMultipartButton);
    //                _contentView.multimodelSearchToggle.gameObject.SetActive(_isOnMultimodelSearchToggle);
    //                _contentView.relatedContentsDropdownToggle.gameObject.SetActive(_isOnRealatedContentToggle);
                });
            }

            void SetScreenshotButtonInteractable(bool isInteractable)
            {
                if (isInteractable)
                {
                    _contentView.screenshotButton.interactable = true;
                    _contentView.screenshotButton.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                }
                else
                {
                    _contentView.screenshotButton.interactable = false;
                    _contentView.screenshotButton.gameObject.GetComponent<CanvasGroup>().alpha = 0.35f;
                } 
            }
            
            void SetBackButtonInteractable(bool isInteractable)
            {
                if (isInteractable)
                {
                    _contentView.backButton.interactable = true;
                    _contentView.backButton.gameObject.GetComponent<Image>().color  = new Color(1, 1, 1, 1);
                }
                else
                {
                    _contentView.backButton.interactable = false;
                    _contentView.backButton.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.35f);
                }
            }
        }

        private void ActionToShowSearchModel() {
            _contentView.multimodelSearchButton.SearchModelPanel.SetActive(true);
            Utilities.Component.SetActiveCanvasGroup(_contentView.ContentMainCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_contentView.ContentSearchModelCanvasGroup, true);
        }

        private void ActionToHideSearchModel() {
            _contentView.multimodelSearchButton.SearchModelPanel.SetActive(false);
            Utilities.Component.SetActiveCanvasGroup(_contentView.ContentMainCanvasGroup, true);
            Utilities.Component.SetActiveCanvasGroup(_contentView.ContentSearchModelCanvasGroup, false);
            
            _contentView.multimodelSearchButton.ClearDropdownToggle();
            _contentViewModel.ClearCachedTextures();
            
            ShowNoSearchResults(false);
        }

        private void ActionToClearSearch() {
            _contentView.multimodelSearchButton.SearchInputField.text = string.Empty;
        }

        private void ActionToSelectSearchInputField() {
            _contentView.multimodelSearchButton.SearchInputField.Select();
        }
    
        #endregion
        
        #region No Search Results

        private void ShowNoSearchResults(bool status)
        {
            _homeModel.ShowNoSearchResults = status;
            _contentView.NoSearchResultsFound.gameObject.SetActive(status);
        }

        #endregion
    }
}