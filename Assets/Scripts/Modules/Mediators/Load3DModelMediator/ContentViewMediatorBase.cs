using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using Module.IK;
using TDL.Models;
using TDL.Server;
using Module.ManagerMaterials;
using TDL.Constants;
using TDL.Services;
using TDL.Signals;
using TMPro;
using TDL.Commands;

namespace TDL.Modules.Model3D
{
    public class ContentViewMediatorBase
    {
        [Inject] protected ContentViewBase.Factory _contentViewFactory;
        [Inject] protected LabelData.Factory labelFactory;
        [Inject] protected ContentModel _contentModel;
        [Inject] protected ContentViewModel _contentViewModel;
        [Inject] protected LocalizationModel _localizationModel;
        [Inject] protected Load3DModelContainer load3DModelContainer;
        [Inject] protected UserLoginModel _userLoginModel;
        [Inject] protected AugmentedRealityMediator _augmentedRealityMediator;
        [Inject] protected Content3DModelHistory _content3DModelHistory;
        [Inject] private HomeModel _homeModel;
        
        [Inject] protected readonly SignalBus _signal;
        [Inject] protected readonly AsyncProcessorService _asyncProcessor;

        [Inject] private ICacheService _cacheService;
        [Inject] private Camera3DModelSettings cameraSettings;
        
        public List<MultiView> MultiView = new List<MultiView>();
        
        public static Action<bool> InitializeARAction = delegate {  };
        public static Action<int> InitializeModuleAction = delegate {  };
        public static Action<bool> ShowContentHomeScreenAction = delegate {  };

        protected bool _isOnArMode;

        #region Description

        protected void SetStudentDescriptionButtonState(Button button, int assetId, string cultureCode)
        {
            bool isActive = _contentModel.HasDescription(assetId, cultureCode, true);
            button.gameObject.SetActive(isActive);
        }

        protected void SetStudentDescriptionButtonListener(Button button, string assetId, string cultureCode, bool isMultiViewSecond = false)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnStudentDescriptionClick(assetId, cultureCode, isMultiViewSecond));
        }

        private void OnStudentDescriptionClick(string assetId, string cultureCode, bool isMultiViewSecond = false)
        {
            if (!_homeModel.OpenedDescriptions.ContainsKey(assetId))
            {
                var asset = _contentModel.GetAssetById(int.Parse(assetId));
            
                var descriptionUrl = asset.Asset.LocalizedStudentDescription.Single(item => item.Culture.Equals(cultureCode)).DescriptionUrl;

                var title = GetAssociatedContentTranslation(int.Parse(assetId), cultureCode);
                _signal.Fire(new GetDescriptionCommandSignal(assetId, string.Empty, title, cultureCode, descriptionUrl, false, isMultiViewSecond));
            }
        }
        
        protected void UpdateDescriptionLanguage(string cultureCode, bool isMultiViewSecond = false)
        {
            _signal.Fire(new GetLanguageChangedDescriptionViewCommandSignal(cultureCode, isMultiViewSecond));
        }

        protected void LoadAndPlayDescription(LoadAndPlayDescriptionViewSignal signal)
        {
            _signal.Fire(new PauseAllExceptActiveDescriptionCommandSignal(signal.Id));
            _signal.Fire(new LoadAndPlayDescriptionCommandSignal(signal.AudioSource, signal.CultureCode, signal.Description));
        }

        protected void PauseAllExceptActiveDescription(PauseAllExceptActiveDescriptionViewSignal signal)
        {
            _signal.Fire(new PauseAllExceptActiveDescriptionCommandSignal(signal.Id));
        }
        
        protected void OnDescriptionBlockModelMovements(OnDescriptionBlockModelMovementsViewSignal signal)
        {
            SetMainModelMovementsBlockStatus(signal.Status);
            SetSecondModelMovementsBlockStatus(signal.Status);
        }
        
        private void SetMainModelMovementsBlockStatus(bool status)
        {
            _contentViewModel.contentViewPC.smoothOrbitCam.enabled = status;
        }
        
        private void SetSecondModelMovementsBlockStatus(bool status)
        {
            if (MultiView.Count > 0)
            {
                MultiView[0].MultimodelView.SmoothOrbitCam.enabled = status;
            }
        }
        
        protected void OnDescriptionClose(OnDescriptionCloseClickViewSignal signal)
        {
            _signal.Fire(new RemoveDescriptionFromArrayCommandSignal(signal.AssetId, signal.LabelId));
        }
        
        protected void CloseAllOpenedDescriptionsOnSecondMultiView()
        {
            _signal.Fire<CloseAllOpenedDescriptionsOnSecondMultiViewCommandSignal>();
        }
        
        protected void CloseAllOpenedDescriptions()
        {
            _signal.Fire<CloseAllOpenedDescriptionsCommandSignal>();
        }

        #endregion
        
        #region Label description

        protected void SetLabelDescriptionButtonState(LabelData label, string cultureCode)
        {
            var foundedLabel = label.LabelLocalNames.SingleOrDefault(localLabel => localLabel.Culture.Equals(cultureCode));
            if (foundedLabel != null)
            {
                label.SetDescriptionIconState(!string.IsNullOrEmpty(foundedLabel.DescriptionUrl));
            }
        }

        protected void SetLabelDescriptionButtonListener(LabelData labelData, string cultureCode)
        {
            labelData.DescriptionButton.onClick.RemoveAllListeners();
            labelData.DescriptionButton.onClick.AddListener(() => OnLabelDescriptionClick(labelData, cultureCode));
        }

        private void OnLabelDescriptionClick(LabelData labelData, string cultureCode)
        {
            if (!_homeModel.OpenedDescriptions.ContainsKey(labelData.LabelId))
            {
                var title = GetAssociatedContentTranslation(labelData.ID, cultureCode)
                            + ": " + GetCurrentTranslationsForItem(labelData.LabelLocalNames.ConvertToLocalName());
            
                var foundedLabel = labelData.LabelLocalNames.SingleOrDefault(item => item.Culture.Equals(cultureCode));
        
                if (foundedLabel != null)
                {
                    _signal.Fire(new GetDescriptionCommandSignal(labelData.ID.ToString(), labelData.LabelId, title, cultureCode, foundedLabel.DescriptionUrl, false, labelData.IsMultiViewSecond));
                }
            }
        }

        #endregion

        internal void LoadBackground(int assetID, RawImage backgroundRawImage)
        {
            if (!_contentModel.HasBackground(assetID))
            {
                backgroundRawImage.gameObject.SetActive(false);
                return;
            }

            _signal.Fire(new DownloadBackgroundCommandSignal(assetID, 
                (isDownloaded, id) =>
                {
                    if (isDownloaded)
                    {
                        var thumbnailPath = _cacheService.GetPathToFile(Path.GetFileName(_contentModel.GetAssetById(id).AssetDetail.AssetContentPlatform.BackgroundUrl));
                        Texture2D texture2D = new Texture2D(1, 1);
                        texture2D.LoadImage(File.ReadAllBytes(thumbnailPath));
                        
                        _signal.Fire(new AddObjectToMemoryManagerSignal(SceneNameConstants.Module3DModel, texture2D));
                        
                        if (backgroundRawImage != null)
                        {
                            backgroundRawImage.texture = texture2D;

                            if (!backgroundRawImage.gameObject.activeSelf)
                            {
                                backgroundRawImage.gameObject.SetActive(true);

                                backgroundRawImage.color = Color.clear;
                                backgroundRawImage.DOColor(Color.white, 1f); 
                            }
                        }
                    }
                }));
        }

        internal bool IsAsset360Model(int assetID)
        {
            return _contentModel.GetAssetById(assetID).Asset.Type.ToLower() == AssetTypeConstants.Type_360_Model;
        }
        
        internal GameObject CreateModel(GameObject prefab, string prefabName, Transform parent)
        {
            var instance = MonoBehaviour.Instantiate(prefab,  parent, false);
            instance.name = prefabName;
            parent.rotation = instance.transform.rotation;
        
            if(parent.gameObject.HasComponent(out RotationAR rotationAr))
            {
                rotationAr.startRotation = parent.rotation.eulerAngles;
            }
        
            instance.transform.localRotation = Quaternion.identity;

            ManagerMaterials.Instance?.ReplaceMaterials(instance);

            return instance;
        }
        
        internal void SetupPopupProgressReplica(int Id, PopupProgress popupProgress)
        {
            PopupProgress pp = popupProgress.Replica(Id);
        
            pp.PopupLabel.text = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.LoadingKey);
            pp.PopupLeftButton.GetComponentInChildren<TextMeshProUGUI>().text = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.CancelKey);
            pp.PopupRightButton.GetComponentInChildren<TextMeshProUGUI>().text = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.CloseKey);

            pp.PopupLeftButton.onClick.RemoveAllListeners();
            pp.PopupLeftButton.onClick.AddListener(() => _signal.Fire(new CancelDownloadProgressCommandSignal(Id)));
            pp.PopupLeftButton.onClick.AddListener(() => pp.SelfDestroy());

            pp.PopupRightButton.onClick.RemoveAllListeners();
            pp.PopupRightButton.onClick.AddListener(() => pp.gameObject.SetActive(false));

            pp.gameObject.SetActive(true);
        }
        
        internal string GetClientAssetModelTranslation(ClientAssetModel model)
        {
            string name;
            if (model.LocalizedName.Count > 1)
                name = GetCurrentTranslationsForItem(model.LocalizedName);
            else
                name = model.LocalizedName[_localizationModel.FallbackCultureCode];
            return name;
        }
        
        internal string GetAssociatedContentTranslation(int assetId, string cultureCode)
        {
            var data = _contentModel.GetAssetById(assetId);

            if (data == null)
            {
                return $"Error. Can't find asset ID: {assetId}";
            }
            
            return data.LocalizedName.ContainsKey(cultureCode)
                ? data.LocalizedName[cultureCode]
                : data.LocalizedName[_localizationModel.FallbackCultureCode];
        }

        internal string GetAssociatedContentTranslation(Dictionary<string, string> localizedName, string cultureCode)
        {
            return localizedName.ContainsKey(cultureCode)
                ? localizedName[cultureCode]
                : localizedName[_localizationModel.FallbackCultureCode];
        }
        
        internal string GetCurrentTranslationsForItem(Dictionary<string, string> itemLocalizedText)
        {
            string cultureCode = _contentViewModel.CurrentContentViewCultureCode;
            string translate = itemLocalizedText.ContainsKey(cultureCode) && !string.IsNullOrEmpty(itemLocalizedText[cultureCode])
                ? itemLocalizedText[cultureCode]
                : itemLocalizedText[_localizationModel.FallbackCultureCode];

            return translate;
        }
        
        internal string GetCurrentTranslationsForItem(LocalName[] itemLocalizedText)
        {
            var text = itemLocalizedText.Distinct().ToList();
            var dictionaryLocale = text.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name);

            var cultureCode = _contentViewModel.CurrentContentViewCultureCode;
            var translate = dictionaryLocale.ContainsKey(cultureCode) && !string.IsNullOrEmpty(dictionaryLocale[cultureCode])
                ? dictionaryLocale[cultureCode]
                : dictionaryLocale[_localizationModel.FallbackCultureCode];

            return translate;
        }

        internal string GetCurrentTranslationsForItem(int assetID, string name)
        {
            var item = GetModel3DLabelItem(assetID, name);

            return GetCurrentTranslationsForItem(item.labelLocal.ConvertToLocalName());
        }
        
        internal string GetAllTranslationsForItem(LocalName[] itemLocalizedText, string cultureCode)
        {
            var dictionaryLocale = itemLocalizedText.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name);

            var translate = dictionaryLocale.ContainsKey(cultureCode) && !string.IsNullOrEmpty(dictionaryLocale[cultureCode])
                ? dictionaryLocale[cultureCode]
                : dictionaryLocale[_localizationModel.FallbackCultureCode];

            return translate;
        }
        
        internal string GetTranslationsForItem(int assetID, string itemName, string cultureCode)
        {
            var item = GetModel3DLabelItem(assetID, itemName);

            if(item == null)
                return string.Empty;

            return GetAllTranslationsForItem(item.labelLocal.ConvertToLocalName(), cultureCode);
        }
    
        internal Assetlabel GetModel3DLabelItem(int _assetID, string labelName)
        {
            var assetLabels = _contentModel.GetAssetById(_assetID).AssetDetail.AssetContentPlatform.assetLabel;
            foreach (var assetLabel in assetLabels)
            {
                var foundedLabel = assetLabel.labelLocal.FirstOrDefault(assetLocale => assetLocale.Name.Equals(labelName));
                if (foundedLabel != null)
                {
                    return assetLabel;
                }
            }

            return null;
        }
        
        public string GetTranslationsForToggle(Dictionary<string, string> itemLocalizedText, string cultureCode)
        {
            string translate = itemLocalizedText.ContainsKey(cultureCode) &&
                               !string.IsNullOrEmpty(itemLocalizedText[cultureCode])
                ? itemLocalizedText[cultureCode]
                : itemLocalizedText[_localizationModel.FallbackCultureCode];

            return translate;
        }

        internal string GetLanguageOnLayer(int layer)
        {
            return _contentViewModel.GetLanguageOnLayer(layer);
        }
        
        internal string GetSystemTranslations(string cultureCode, string key)
        {
            return _contentViewModel.GetSystemTranslations(cultureCode, key);
        }
        
        internal string GetCurrentSystemTranslations(string key)
        {
            return _contentViewModel.GetCurrentSystemTranslations(key);
        }
        
        internal List<(Toggle toggle, string cultureCode)> CreateLanguagesDropdownToggle(DropdownToggle dropdownToggle)
        {
            List<(Toggle toggle, string cultureCode)> dropdownLst = new List<(Toggle toggle, string cultureCode)>();
        
            dropdownToggle.ClearDropdownToggle();
        
            foreach (LanguageResource l in _localizationModel.AvailableLanguages)
            {
                Toggle tgl = dropdownToggle.CreateTemplate<Toggle>(l.Name);
                tgl.group = dropdownToggle.ToggleGroupContainer;
                dropdownLst.Add((tgl, l.Culture));
            }
        
            dropdownToggle.SetAllowSwitchOff(false);
            dropdownToggle.Template.SetActive(false);

            return dropdownLst;
        }
        
        internal void LinkModelsToToggles(Dictionary<string, ObjectHighlighter> _models, Dictionary<string, Toggle> _labelToggles)
        {
            if (_labelToggles.Count == 0)
                return;

            foreach (var toggleName in _labelToggles.Keys)
            {
                if (_models.ContainsKey(toggleName))
                {
                    _models[toggleName].SetToggle(_labelToggles[toggleName]);
                }
            } 
        }

        internal void CheckSelectAllActive(Toggle selectAll)
        {
            List<Toggle> tgls = new List<Toggle>(selectAll.transform.parent.GetComponentsInChildren<Toggle>(true));

            tgls.Remove(selectAll);

            if (tgls.FindAll(tgl => tgl.isOn == false).Count == 0)
            {
                selectAll.SetValue(true, false);
            }
            else
            {
                selectAll.SetValue(false, false);
            }
        }
        
        internal void DisposeModule(Action disposedAction = null)
        {
            if(DeviceInfo.IsPCInterface())
                _signal.Fire(new StartVideoRecordingSignal(false, _contentViewModel.mainAssetID,  _contentViewModel.contentViewPC.recorderToggle));

            _asyncProcessor.Wait(0, () =>
            {
                CloseAllOpenedDescriptions();

                _contentViewModel.ClearCachedTextures();
                load3DModelContainer.CloseModule();
                
                if(DeviceInfo.IsPCInterface())
                    _contentViewModel.contentViewPC.CloseView();
                
                _content3DModelHistory.ClearHistory();
                _asyncProcessor.Wait(0,() => disposedAction?.Invoke());
            });
        }

        private string currentScreenLabel;
        private Toggle currentScreenLabelToggle;
        internal void SetActiveScreenLabel(bool status, ContentViewMobile contentViewMobile, Toggle toggle = null, string labelItem = null, bool updateTextCurrToggle = false)
        {
            contentViewMobile.screenLabel.SetActive(status);

            if (status && labelItem != null || updateTextCurrToggle)
            {
                if(!updateTextCurrToggle)
                    currentScreenLabel = labelItem;

                if (toggle)
                    currentScreenLabelToggle = toggle;

                if (currentScreenLabelToggle)
                {
                    TextMeshProUGUI tmp = currentScreenLabelToggle.GetComponentInChildren<TextMeshProUGUI>();
            
                    string labeltext = tmp.text;

                    contentViewMobile.screenLabel.GetComponentInChildren<TextMeshProUGUI>().text = labeltext;
                }
            }
        }
        
        internal void SetAllLabelActive(bool isActive)
        {
            _contentViewModel.contentViewPC.labelsListDropdownToggle.SetAllToggleIsOn(isActive);
        }

        protected void SetStudentNoteVisibility(ContentViewBase _contentViewBase)
        {
            _contentViewBase.SetStudentNoteVisibility(!_userLoginModel.IsTeacher);
            _contentViewBase.studentNoteButton.onClick.AddListener(ShowStudentNotes);
        }

        private void ShowStudentNotes()
        {
            _signal.Fire(new ShowStudentNotesPanelViewSignal(_contentViewModel.mainAssetID, _contentViewModel.CurrentContentViewCultureCode));
        }
        
        internal void AddIKView(GameObject model, int assetID)
        {
            if (_contentModel.GetAssetById(assetID).Asset.Type.ToLower() == AssetTypeConstants.Type_Rigged_Model)
            {
                model.AddComponent<Module.IK.IKView>();
            }

            var name = _contentModel.SelectedAsset.Asset.Name;
            switch (name)
            {
                case "Rigged-Knee Joint animated":
                    model.AddComponent<Module.IK.IKView>();
                    break;
                case "Rigged-Shoulder and Elbow animated":
                    model.AddComponent<Module.IK.IKView>();
                    break;
                case "Rigged-Joint animated":
                    model.AddComponent<Module.IK.IKView>();
                    break;
            }
            
            _asyncProcessor.Wait(0, () => model.transform.parent.SetLayer(model.transform.parent.gameObject.layer));
        }

        internal void SetPivotPoint(GameObject go, int assetId)
        {
            // Set Pivot Point to Center Of Mass
            Vector3 pos = Vector3.zero;
            if (_contentModel.GetAssetById(assetId).Asset.Type.ToLower() != AssetTypeConstants.Type_Rigged_Model)
            {
                if(go.HasComponent(out MeshFilter mf))
                    pos = mf.mesh.GetCenterOfMass();
            }
            else
            {
                if (go.HasComponent(out SkinnedMeshRenderer smr))
                {
                    Vector3 modelScale = go.transform.localScale;
                    go.transform.localScale = Vector3.one;
                    Mesh newMesh = new Mesh();
                    smr.BakeMesh(newMesh);
                    go.AddComponent<MeshFilter>().mesh = newMesh;
                    go.AddComponent<MeshRenderer>().materials = smr.materials;
                    MonoBehaviour.Destroy(smr);
                    pos = newMesh.GetCenterOfMass();
                    go.transform.localScale = modelScale;
                }
            }

            go.transform.localPosition = pos;
        }
        
        public void CameraResetBase(SmoothOrbitCam smoothOrbitCam, bool autoZoom = false, bool applyNewZoomDistance = false, bool selfTarget = false)
        {
            _asyncProcessor.Wait(0, Reset);

            void Reset()
            {
                if(!smoothOrbitCam)
                    return;
                
                smoothOrbitCam.gameObject.GetAllInSceneOnLayer<RelocatorView>().ForEach(rl => rl.ResetPosition());

                _asyncProcessor.Wait(0, () => { IKView.UpdateMeshColliderAction?.Invoke(); });
                
                smoothOrbitCam.enabled = true;

                var _camPos = smoothOrbitCam.target.GetChildrenByName("CameraPosition");
                
                if (_camPos != null)
                {
                    if (selfTarget)
                    {
                        smoothOrbitCam.target = _camPos.transform;
                        smoothOrbitCam.distanceMin = 0;
                        smoothOrbitCam.distance = 0;
                    }
                  
                    smoothOrbitCam.transform.position = _camPos.transform.position;
                    smoothOrbitCam.transform.LookAt(smoothOrbitCam.target.transform);
                    
                    smoothOrbitCam.SetDefaultValue(
                        Vector3.Distance(_camPos.transform.position, smoothOrbitCam.target.transform.position),
                        _camPos.transform.position,
                        smoothOrbitCam.transform.eulerAngles);
                }
                else
                {
                    smoothOrbitCam.transform.position = smoothOrbitCam.GetMyCamera().transform.position;
                    smoothOrbitCam.transform.LookAt(smoothOrbitCam.target.transform);

                    smoothOrbitCam.SetDefaultValue(cameraSettings.Distance,
                        Vector3.zero,
                        Vector3.zero);
                }
                
                smoothOrbitCam.ResetMainValues();

                if (autoZoom || DeviceInfo.IsTablet() && DeviceInfo.IsScreenPortrait())
                {
                    smoothOrbitCam.AutoZoomOnTarget(applyNewZoomDistance);
                }

                smoothOrbitCam.xSpeed = smoothOrbitCam.ySpeed = cameraSettings.RotationSpeed;
            }
        }
    }
}