using System;
using System.Collections.Generic;
using System.Linq;
using TDL.Commands;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using BestHTTP;
using TDL.Constants;
using TDL.Views;

namespace TDL.Modules.Model3D
{
    public class ContentViewMobileMediator : ContentViewMediatorBase, IInitializable, IDisposable
    {
        public ContentViewMobile _contentView => _contentViewModel.contentViewMobile;

        //private int _assetID;
        private Dictionary<string, Toggle> _labelToggles = new Dictionary<string, Toggle>();
        private Dictionary<string, LabelData> _labels = new Dictionary<string, LabelData>();
        private Dictionary<string, ObjectHighlighter> _models = new Dictionary<string, ObjectHighlighter>();

        private Dictionary<string, TextMeshProUGUI> _associatedContentsTexts = new Dictionary<string, TextMeshProUGUI>();
        
        public void Initialize()
        {
            _contentViewModel.UpdateCurrentLanguage();

            _contentViewModel.mainAssetID = _contentModel.SelectedAsset.Asset.Id;
            _contentViewModel.contentViewMobile = (ContentViewMobile)_contentViewFactory.Create();
            _contentViewModel.contentViewMobile.InitUiComponents();
            _contentView.SubscribeOnListeners();
            
            _contentView.backButton.onClick.AddListener(DisposeModule);
            _contentView.multimodelSearchButton.SearchInputField.onEndEdit.AddListener(SearchIfAllowed);
            _contentView.SearchGoButton.onClick.AddListener(() =>
            {
                string searchValue = _contentView.multimodelSearchButton.SearchInputField.text;

                if(!searchValue.Equals(string.Empty))
                    MultimodelSearch(searchValue);
            });

            _contentView.multimodelSearchButton.GetComponent<Toggle>().onValueChanged.AddListener(OnShowSearchPanel);
            
            // Reset camera position, zoom and rotation
            _contentViewModel.contentViewMobile.resetButton.onClick.AddListener(() =>  ResetCamera());

            _contentViewModel.contentViewMobile.executionEvents.onRectTransformDimensionsChange.AddListener(() => ResetCamera());

            SetStudentNoteVisibility(_contentView);

            _contentView.gameObject.SetActive(false);
            _contentView.multimodelSearchButton.gameObject.SetActive(false);

            // search
            _contentView.multimodelSearchButton.SearchInputField.onValueChanged.AddListener(OnSearchInputChanged);
            
            // video recording
            _contentView.recorderToggle.onValueChanged.AddListener(OnRecorder);
        }

        private void ResetCamera()
        {
            bool is360 = IsAsset360Model(_contentViewModel.mainAssetID);
            CameraResetBase(_contentViewModel.contentViewMobile.smoothOrbitCam, !is360, selfTarget: is360);
        }
        
        private void OnSearchInputChanged(string searchValue)
        {
            _contentView.SearchGoButton.interactable = _contentView.multimodelSearchButton.SearchInputField.text.Length >= SearchConstants.MinimumSearchSymbols;
        }

        private void SearchIfAllowed(string search)
        {
            if (IsSearchAllowed())
            {
                MultimodelSearch(search);
            }
        }
        
        private bool IsSearchAllowed()
        {
            return _contentView.SearchGoButton.interactable;
        }

        private void MultimodelSearch(string search)
        {
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
                    _contentView.NoSearchResultsFound.gameObject.SetActive(true);
                    return;
                }
                
                _contentView.NoSearchResultsFound.gameObject.SetActive(false);

                foreach (ClientAssetModel clientAssetModel in result)
                {
                    var localizedName = GetClientAssetModelTranslation(clientAssetModel);
                    Sprite sprite = _contentViewModel.GetSpriteType(clientAssetModel.Asset.Type);
                    
                    Button btn = _contentView.multimodelSearchButton.CreateTemplate(localizedName, sprite)
                        .GetComponentInChildren<Button>();

                    btn.onClick.AddListener(() =>
                    {
                        _contentView.multimodelSearchButton.DropdownListToggle.SetValue(false, true);
                        _contentView.multimodelSearchButton.gameObject.SetActive(false);
                        //_contentView.multimodelSearchToggle.SearchInputField.text = "";
                        _signal.Fire(new ShowDebugLogCommandSignal($"Add Model to AR: {clientAssetModel.Asset.Id}"));
                        Create(clientAssetModel.Asset.Id);
                    });

                    void Create(int modelId)
                    {
                        Slider slider = _contentView.cancelLoading.GetComponentInChildren<Slider>(true);
                        
                        _contentModel.AssetDetailsSignalSource = new DownloadAssetCommandSignal(modelId, MultimodelDownloadAsset);
                        _signal.Fire(new Module3dStartAssetDetailsCommandSignal(modelId));
                        
                        //_signal.Fire(new DownloadAssetCommandSignal(modelId, MultimodelDownloadAsset));

                        _contentView.cancelLoading.onClick.RemoveAllListeners();
                        _contentView.cancelLoading.onClick.AddListener(() =>
                            _signal.Fire(new CancelDownloadProgressCommandSignal(modelId)));
                        _contentView.cancelLoading.onClick.AddListener(() =>
                            _contentView.multimodelSearchButton.gameObject.SetActive(true));
                        _contentView.cancelLoading.onClick.AddListener(() =>
                            _contentView.cancelLoading.gameObject.SetActive(false));

                        void MultimodelDownloadAsset(bool isDownloaded, int downloadedID, HTTPRequestStates downloadState, float downloadProgress)
                        {
                            if (isDownloaded)
                            {
                                _contentViewModel.secondAssetID = downloadedID;
                                _augmentedRealityMediator.AddSecondModel(downloadedID);

                                _contentView.multimodelSearchButton.gameObject.SetActive(true);
                                _contentView.cancelLoading.gameObject.SetActive(false);
                            }
                            else
                            {
                                switch (downloadState)
                                {
                                    case HTTPRequestStates.Processing:
                                        _contentView.cancelLoading.gameObject.SetActive(true);

                                        slider.value = downloadProgress;

                                        break;

                                    case HTTPRequestStates.Error:
                                        _contentView.cancelLoading.gameObject.SetActive(false);

                                        _signal.Fire(new SendCrashAnalyticsCommandSignal("@@@ Load Asset Error /: Error"));

                                        break;

                                    case HTTPRequestStates.ConnectionTimedOut:
                                        _contentView.cancelLoading.gameObject.SetActive(false);

                                        _signal.Fire(new SendCrashAnalyticsCommandSignal("@@@ Download Asset Error /: Connection TimedOut"));

                                        break;
                                }
                            }
                        }
                    }

                    _signal.Fire(new LoadThumbnailsCommandSignal(clientAssetModel.Asset, btn.targetGraphic as Image));
                }
            }
        }

        public void InitializeModule(GameObject go)
        {
            _contentView.gameObject.SetActive(true);
            _contentView.smoothOrbitCam.distanceMax = 25f;
            _contentView.smoothOrbitCam.SetZoomDistance(15);
            SetActiveScreenLabel(false, _contentView);
        
            CreateLabelList();
            CreateModelPrefab(go);
            LinkModelsToToggles(_models, _labelToggles);
            CreateLanguagesDropdownToggle();
            InitializeRelatedContent();
            ShowReturnButton();
            ShowPlayAnimationButton(go);
            LoadBackground(_contentViewModel.mainAssetID, _contentView.backgroundRawImage);
            
            _signal.Fire(new StartAugmentedRealitySignal(isInit =>
            {
                _isOnArMode = isInit;
                _contentView.multimodelSearchButton.gameObject.SetActive(_isOnArMode);

                if (isInit)
                    UpdateTopRightMenuUi();
            }));

            _asyncProcessor.Wait(0, () => ResetCamera());
        
            if(Application.isEditor)
                _contentView.multimodelSearchButton.gameObject.SetActive(true); // Test    AR     <<<<<
        }
        
        private void ShowReturnButton()
        {
            if (_content3DModelHistory.ContainItems())
            {
                _contentView.returnButton.gameObject.SetActive(true);
                _contentView.backButton.gameObject.SetActive(false);
                
                var index = _content3DModelHistory.GetLastItem();
                _contentView.returnButton.onClick.RemoveAllListeners();
                _contentView.returnButton.onClick.AddListener(() => OnAssociatedContentOnNewViewClick(index));
                _contentView.returnButton.onClick.AddListener(() => _content3DModelHistory.UndoHistory());
            }
            else
            {
                _contentView.returnButton.gameObject.SetActive(false);
                _contentView.backButton.gameObject.SetActive(true);
            }

            _content3DModelHistory.AddHistory(_contentViewModel.mainAssetID);
        }

        private void DisposeModule()
        {
            _contentViewModel.ClearCachedTextures();
            _contentView.CloseView();
            _content3DModelHistory.ClearHistory();
        }

        private void CreateLabelList()
        {
            if (_contentModel.SelectedAsset.AssetDetail.AssetContentPlatform.assetLabel == null 
            || _contentModel.SelectedAsset.AssetDetail.AssetContentPlatform.assetLabel.Length == 0)
            {
                _contentView.labelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(false);
                return;
            }

            _contentView.labelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(true);

            int i = 0;
            foreach (var v in _contentModel.SelectedAsset.AssetDetail.AssetContentPlatform.assetLabel)
            {
                i++;
                ColorUtility.TryParseHtmlString(v.highlightColor, out Color newCol);
                
                string itemKey = v.labelLocal
                    .First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;

                LabelData ld = labelFactory.Create(i.ToString(), newCol);
                ld.ID = _contentViewModel.mainAssetID;
                ld.LabelId = ld.ID + "_" + v.labelId;
                ld.LabelLocalNames = v.labelLocal;
                ld.SetModelPartName(itemKey);
                    
                ld.transform.position = new Vector3(v.position.x, v.position.y, v.position.z);
                ld.transform.rotation = Quaternion.Euler(v.rotation.x, v.rotation.y, v.rotation.z);

                Toggle tgl = _contentView.labelsListDropdownToggle.CreateLabelToggle($"{i}. {itemKey}", ld.gameObject, false);
                tgl.gameObject.name = itemKey;

                switch (ld)
                {
                    case LabelDataMobile ldm:
                        ldm.myToggleText = tgl.GetComponentInChildren<TextMeshProUGUI>(true);
                        break;
                }
                
                _labels.Add(itemKey, ld);
                _labelToggles.Add(itemKey, tgl);
                
                tgl.onValueChanged.AddListener(value => ld.GetComponent<LabelLine>().SetLabelLineGameObjectActive(value));

                tgl.onValueChanged.AddListener((value) => SetActiveScreenLabel(value, _contentView, tgl, itemKey));
            }
            
            _contentView.labelsListDropdownToggle.Template.SetActive(false);
        }

        private void CreateLanguagesDropdownToggle()
        {
            foreach (LanguageResource l in _localizationModel.AvailableLanguages)
            {
                _contentView.languageCanvasPanel.SetPanelName("Language");
                _contentView.languageCanvasPanel.EnableLeftButton(false);
                _contentView.languageCanvasPanel.SetRightButtonName("Ok");
                _contentView.languageCanvasPanel.rightButton.onClick.AddListener(_contentView.languageCanvasPanel.CloseCanvasPanel);
                
                Toggle tgl = _contentView.languageCanvasPanel.contentPanel.CreateButton(l.Name) as Toggle;
                
                tgl.onValueChanged.AddListener(isOn => { ChangeLanguageByToggle(isOn, l.Culture); });
                tgl.onValueChanged.AddListener(isOn => { _contentView.languageButton.GetComponentInChildren<TextMeshProUGUI>().SetText(l.Name); });
                
                if (l.Culture == _contentViewModel.CurrentContentViewCultureCode)
                    tgl.SetValue(true, true);
            }
        }
        
        public void OnRecorder(bool isRecording) 
        {
            _signal.Fire(new StartVideoRecordingSignal(isRecording, _contentViewModel.mainAssetID, _contentView.recorderToggle, _contentViewModel.CurrentContentViewCultureCode));

            if (isRecording)
            {
                if (!DeviceInfo.IsMobile())
                {
                    _contentView.screenshotButton.interactable = false;
                }

                _contentView.backButton.interactable = false;
            }
            else
            {
                _asyncProcessor.Wait(0, () =>
                {
                    if (!DeviceInfo.IsMobile())
                    {
                        _contentView.screenshotButton.interactable = true;
                    }
                    
                    _contentView.backButton.interactable = true;
                });
            }
        }

        #region Related Content

        private void InitializeRelatedContent()
        {
            if (HasRelatedContent())
            {
                SetRelatedContentVisibility(true);
                CreateAssociatedContentsDropdownToggle();
            }
            else
            {
                SetRelatedContentVisibility(false);
            }
        }
        
        private void SetRelatedContentVisibility(bool status)
        {
            if (status)
            {
                _contentView.relatedCanvasPanel.SetPanelName("Related Content");
                _contentView.relatedCanvasPanel.EnableLeftButton(false);
                _contentView.relatedCanvasPanel.SetRightButtonName("Cancel");
                _contentView.relatedCanvasPanel.onEnable.AddListener(_contentView.relatedCanvasPanel.onHide.RemoveAllListeners);
                _contentView.relatedCanvasPanel.rightButton.onClick.AddListener(_contentView.relatedCanvasPanel.CloseCanvasPanel);
            }
            else
            {
                _contentView.relatedContentButton.gameObject.SetActive(false);
            }
        }

        private bool HasRelatedContent()
        {
            var associatedContents = _contentModel.SelectedAsset.AssetDetail.AssociatedContents;
            return !(associatedContents == null || associatedContents.Length == 0);
        }

        private ContentPanel GetRelatedContentPanel()
        {
            return _contentView.relatedCanvasPanel.contentPanel;
        }

        private void CreateAssociatedContentsDropdownToggle()
        {
            var associatedContents = _contentModel.SelectedAsset.AssetDetail.AssociatedContents;
            var contentPanel = GetRelatedContentPanel();

            contentPanel.ClearCachedButtonsWithId();
            
            foreach (var ac in associatedContents)
            {
                var data = _contentModel.GetAssetById(ac.AssetId);

                if (data != null)
                {
                    Button btn;
                    if (data.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode))
                    {
                        btn = contentPanel.CreateButton(
                            data.LocalizedName[_localizationModel.CurrentLanguageCultureCode], data.Asset.Id) as Button;
                    }
                    else
                    {
                        btn = contentPanel.CreateButton(data.LocalizedName[_localizationModel.FallbackCultureCode], data.Asset.Id) as
                            Button;
                    }

                    if (_associatedContentsTexts.ContainsKey(ac.Name))
                    {
                        _signal.Fire(new SendCrashAnalyticsCommandSignal("Already added associated content: " + ac.Name + ", assetID: " + _contentViewModel.mainAssetID));
                    }
                    else
                    {
                        _associatedContentsTexts.Add(ac.Name, btn.GetComponentInChildren<TextMeshProUGUI>());
                    }
                
                    btn.onClick.AddListener(_contentView.relatedCanvasPanel.onHide.RemoveAllListeners);
                    btn.onClick.AddListener(_contentView.relatedCanvasPanel.CloseCanvasPanel);


                    if (ac.Type.ToLower().Equals(AssetTypeConstants.Type_2D_Video)
                        || ac.Type.ToLower().Equals(AssetTypeConstants.Type_3D_Video))
                    {
                        btn.onClick.AddListener(() =>
                        {
                            _contentView.relatedCanvasPanel.onHide.AddListener(() =>
                                OnAssociatedContentClick(data.Asset.Id));
                        });
                    }
                    else
                    {
                        btn.onClick.AddListener(() =>
                            _contentView.relatedCanvasPanel.onHide.AddListener(() =>
                                OnAssociatedContentOnNewViewClick(data.Asset.Id)));
                    }
                }
                else
                {
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("Missing Associated Content ID: " + ac.Id + " -- Name: " + ac.Name + " | Model id = " + ac.ModelId));
                }
            }

            if (_associatedContentsTexts.Count == 0)
            {
                _contentView.relatedContentButton.gameObject.SetActive(false);
            }
        }
        
        private void UpdateRelatedContentTranslations()
        {
            var associatedContents = _contentModel.SelectedAsset.AssetDetail.AssociatedContents;
            var contentPanel = GetRelatedContentPanel();

            foreach (var content in associatedContents)
            {
                if (contentPanel.CachedButtonsWithId.ContainsKey(content.AssetId))
                {
                    var asset = _contentModel.GetAssetById(content.AssetId);
                    contentPanel.CachedButtonsWithId[content.AssetId].text = asset.LocalizedName.ContainsKey(_contentViewModel.CurrentContentViewCultureCode)
                        ? asset.LocalizedName[_contentViewModel.CurrentContentViewCultureCode]
                        : asset.LocalizedName[_localizationModel.FallbackCultureCode];
                }
            }
        }

        private void OnAssociatedContentClick(int Id)
        {
            _contentModel.AssetDetailsSignalSource = new AssetItemClickCommandSignal(Id, -1);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {Id}));
        }

        private void OnAssociatedContentOnNewViewClick(int Id)
        {
            DisposeRelatedContent();
            _contentView.CloseView();
            load3DModelContainer.UnloadModule();
            
            _contentModel.AssetDetailsSignalSource = new AssetItemClickCommandSignal(Id, -1);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {Id}));
        }
        
        private void DisposeRelatedContent()
        {
            if (HasRelatedContent())
            {
                var relatedContentPanel = GetRelatedContentPanel();
                relatedContentPanel.CachedButtonsWithId.Clear();
            }
        }
        
        #endregion

        #region Language 

        private void ChangeLanguageByToggle(bool isThis, string cultureCode)
        {
            if (isThis)
            {
                int i = 0;
                int layer = _models.Count > 0 ? _models.First().Value.gameObject.layer : _contentView.gameObject.layer;
                _contentViewModel.SetLanguageOnLayer(layer, cultureCode, true);

                var mainAsset = _contentModel.GetAssetById(_contentViewModel.mainAssetID);
                
                if (mainAsset.LocalizedName.Count > 1)
                    _contentView.SetLabel(GetTranslationsForToggle(mainAsset.LocalizedName, cultureCode));
                else
                    _contentView.SetLabel(mainAsset.LocalizedName[_localizationModel.FallbackCultureCode]);

                UpdateTopRightMenuUi();
                SetLanguageTextInModel(cultureCode);
                
                // color Piker
                var activate = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.ActivateKey);
                _contentView.colorPickerControl.transform.parent.GetComponentByName<TextMeshProUGUI>("ActivateColorPikerLabel").text = activate;
                
                // notes
                var notes = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.NotesKey).ToUpper();
                _contentView.studentNoteButton.GetComponentInChildren<TextMeshProUGUI>().text = notes;
                
                if (mainAsset.AssetDetail.AssetContentPlatform.assetLabel != null)
                {
                    TranslateLabelDropdown(cultureCode, _contentViewModel.mainAssetID);
                    _contentView.labelsListDropdownToggle.SortToggleItemByText(isMobileLabel: true);
                }

                if (_contentViewModel.secondAssetID > 0)
                {
                    if (_contentModel.GetAssetById(_contentViewModel.secondAssetID).AssetDetail.AssetContentPlatform.assetLabel != null)
                        TranslateLabelDropdown(cultureCode, _contentViewModel.secondAssetID);
                    _contentView.labelsListDropdownToggle.SortToggleItemByText(isMobileLabel: true);

                }

                if (_contentView.screenLabel.activeSelf)
                {
                    _asyncProcessor.Wait(0, () =>
                    {
                        SetActiveScreenLabel(true, _contentView, updateTextCurrToggle: true); 
                    });
                }
                
                if (HasRelatedContent())
                {
                    UpdateRelatedContentTranslations();
                }
                
                LabelDataMobile.UpdateMobileLabelAction?.Invoke();
            }
        }

        private void TranslateLabelDropdown(string cultureCode, int id)
        {
            foreach (Transform item in _contentView.labelsListDropdownToggle.ToggleGroupContainer.transform)
            {
                string translate;
                if (_contentModel.GetAssetById(id).LocalizedName[_localizationModel.FallbackCultureCode].Equals(item.name))
                {
                    translate = $"> {GetTranslationsForToggle(_contentModel.GetAssetById(id).LocalizedName, cultureCode)}";
                }
                else
                {
                    translate = GetTranslationsForItem(id, item.name, cultureCode);
                }
                
                if (!string.IsNullOrEmpty(translate))
                {
                    TextMeshProUGUI tmp = item.GetComponentInChildren<TextMeshProUGUI>();

                    string numberString = tmp.text.Split('.')[0];

                    if (int.TryParse(numberString, out int labelNumber))
                        tmp.text = $"{labelNumber}. {translate}";
                    else
                        tmp.text = $"{translate}";
                }
            }
        }

        private void UpdateTopRightMenuUi()
        {
            _contentView.multimodelSearchButton.SearchInputField.placeholder.GetComponent<Text>().text =
                GetCurrentSystemTranslations(LocalizationConstants.PlaceholderSearchKey);
            
            _contentView.multimodelSearchButton.GetComponentInChildren<TextMeshProUGUI>().text =
                GetCurrentSystemTranslations(LocalizationConstants.AddModelKey);

            _contentView.relatedCanvasPanel.SetPanelName(GetCurrentSystemTranslations(LocalizationConstants.RelatedContentKey));
            _contentView.relatedCanvasPanel.SetRightButtonName(GetCurrentSystemTranslations(LocalizationConstants.CancelKey));
            
            _contentView.languageCanvasPanel.SetPanelName(GetCurrentSystemTranslations(LocalizationConstants.SelectLanguageKey));
            _contentView.languageCanvasPanel.SetRightButtonName(GetCurrentSystemTranslations(LocalizationConstants.OkKey));
            
            _contentView.colorPikerToggle.GetComponentInChildren<TextMeshProUGUI>().SetText(GetCurrentSystemTranslations(LocalizationConstants.BackgroundColorKey));
            _contentView.arCameraToggle.GetComponentInChildren<TextMeshProUGUI>().SetText(GetCurrentSystemTranslations(LocalizationConstants.ArModeKey));
            
            _contentView.NoSearchResultsFound.text = 
                _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.NoSearchResultsKey);
            
            string defaultPositionKey;
            if (_augmentedRealityMediator.IsRunAR())
            {
                defaultPositionKey = GetCurrentSystemTranslations(LocalizationConstants.DefaultPositionARKey);
            }
            else
            {
                defaultPositionKey = GetCurrentSystemTranslations(LocalizationConstants.DefaultPositionKey);
            }
            
            _contentView.resetButton.GetComponentInChildren<TextMeshProUGUI>().SetText(defaultPositionKey);

            _contentView.fullscreenToggle.transform.Get<TextMeshProUGUI>("Text_Panel/Show").SetText(GetCurrentSystemTranslations(LocalizationConstants.ShowInterfaceKey).ToUpper());
            _contentView.fullscreenToggle.transform.Get<TextMeshProUGUI>("Text_Panel/Hide").SetText(GetCurrentSystemTranslations(LocalizationConstants.HideInterfaceKey).ToUpper());

            foreach (TextMeshProUGUI labelText in _contentView.labelsListText)
            {
                labelText.SetText(GetCurrentSystemTranslations(LocalizationConstants.LabelsKey));
            }
        }

        #endregion
        
        #region No Search Results

        private void OnShowSearchPanel(bool isShow)
        {
            if (isShow)
            {
                _contentView.NoSearchResultsFound.gameObject.SetActive(false);
            }
            else
            {
                _contentView.multimodelSearchButton.ClearDropdownToggle();
                _contentViewModel.ClearCachedTextures();
                _contentView.multimodelSearchButton.SearchInputField.text = string.Empty;
            }
        }

        #endregion

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
            
            load3DModelContainer.transform.SetLayer(_contentView.gameObject.layer);

            AddIKView(instance, _contentViewModel.mainAssetID);
        }

        private void SetLanguageTextInModel(string cultureCode)
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
                    if (language_EN.name.Equals(cultureCode))
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
                    if (language_NO.name.Equals(cultureCode))
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

        public void Dispose()
        {
            DisposeRelatedContent();
            
            if(_userLoginModel.IsLoggedAsUser)
                _signal.Fire<ShowHomeScreenCommandSignal>();
            else
            {
                _signal.Fire<ShowLoginScreenCommandSignal>();
            }
            
            // search
            _contentView.multimodelSearchButton.SearchInputField.onValueChanged.RemoveAllListeners();
        }
    }
}