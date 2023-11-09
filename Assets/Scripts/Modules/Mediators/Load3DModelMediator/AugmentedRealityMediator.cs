using System;
using System.Collections.Generic;
using System.Linq;
using Module.IK;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using TDL.Views;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if !UNITY_WEBGL
using Vuforia;
#endif
using Zenject;
using Image = UnityEngine.UI.Image;

namespace TDL.Modules.Model3D
{
    public class StartAugmentedRealitySignal : ISignal
    {
        public Action<bool> Callback;
    
        public StartAugmentedRealitySignal(Action<bool> callback)
        {
            Callback = callback;
        }
    }
    
    public class AugmentedRealityMediator : ContentViewMediatorBase, IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;

        [Inject] private Load3DModelContainer _load3DModelContainer;
        [Inject] private ScreenContainer _screenContainer;
        [Inject] private ContentModel _contentModel;
        [Inject] private ContentViewModel _contentViewModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private LabelData.Factory _labelFactory;

        private bool _isActiveAR;
        private bool _isFirstInit;
        private bool _isSupportedAR;

        private Action<bool> _callback;
        
#if !UNITY_WEBGL
        private AnchorBehaviour _anchorBehaviour;
        
        private SmartTerrain _smartTerrain;
        private PositionalDeviceTracker _positionalDeviceTracker;

        
        private Camera _vuforiaCamera;
#endif
         
       
        
        private (GameObject planeFinder, TranslationAR translationAr, RotationAR rotationAr)[] _planeAR = {(null, null, null), (null, null, null)};

        private static Vector3 GetScaleAR => DeviceInfo.IsMobile() ? Vector3.one / 6f : Vector3.one / 9f;
        
        private const string UnsupportedDeviceTitle = "Unsupported Device";
        private const string UnsupportedDeviceBody =
            "This device has failed to start the Positional Device Tracker. " +
            "Please check the list of supported Ground Plane devices on our site: " +
            "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";
        
        private void StartAR(StartAugmentedRealitySignal signal)
        {
            _isActiveAR = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.ARmodeSettings);
            
            if (_isFirstInit)
                return;

            _countStarted = 0;
            _isFirstInit = true;
            _callback = signal.Callback;
            _isSupportedAR = false;
            _callback.Invoke(false);

            if (_isActiveAR)
            {
                _signal.Fire(new PopupOverlaySignal(true, "Initialization AR"));
                
                _signal.Fire(new CheckCameraPermissionCommandSignal(isAccepted =>
                {
                    _signal.Fire(new PopupOverlaySignal(false));

                    if (isAccepted)
                    {
                        Start(signal);
                    }
                }));
            }
        }

        private void Start(StartAugmentedRealitySignal signal)
        {
            if (DeviceInfo.IsMobile() || DeviceInfo.IsTablet() || Application.isEditor)
            {
                if (_isActiveAR)
                {
                    _signal.Fire(new PopupOverlaySignal(true, "Initialization AR"));

                    FirstInitVuforia();

                    SetObjectActive(false);
#if !UNITY_WEBGL
                    if (_vuforiaCamera == null)
                    {
                        _vuforiaCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
                        _vuforiaCamera.enabled = true;
                    }
#endif

                    if (!Application.isEditor)
                    {
                        if (_contentViewModel.contentViewMobile)
                            _planeAR[0].planeFinder = MonoBehaviour.Instantiate(_screenContainer.VuforiaPlaneFinder,
                                _contentViewModel.contentViewMobile.cameraOrigin.parent);

                        if (_contentViewModel.contentViewPC)
                            _planeAR[0].planeFinder = MonoBehaviour.Instantiate(_screenContainer.VuforiaPlaneFinder,
                                _contentViewModel.contentViewPC.cameraOrigin.parent);
                    }

                    _asyncProcessor.Wait(2f, VuforiaStarted);
                }
            }
        }

        private void FirstInitVuforia()
        {
#if !UNITY_WEBGL
            if (_isActiveAR)
            {
                if (!Application.isEditor)
                {

                    VuforiaRuntime.Instance.RegisterVuforiaInitErrorCallback(OnVuforiaInitializationError);
                    VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);

                }
            }

            if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.NOT_INITIALIZED)
                VuforiaRuntime.Instance.InitVuforia();

            if (VuforiaBehaviour.Instance == null)
            {
                GameObject arCamera = MonoBehaviour.Instantiate(_screenContainer.VuforiaARCamera);
                SceneManager.MoveGameObjectToScene(arCamera, _load3DModelContainer.gameObject.scene);
                MonoBehaviour.DontDestroyOnLoad(VuforiaBehaviour.Instance);
            }
#endif
        }

        private int _countStarted;
        private void VuforiaStarted()
        {
#if !UNITY_WEBGL
            if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZED 
                && VuforiaARController.Instance.HasStarted || Application.isEditor)
            {
                VuforiaActivate();
            }
            else
            {
                if (_countStarted < 10)
                {
                    _asyncProcessor.Wait(0.2f, VuforiaStarted);
                    _countStarted++;
                }
                else
                {
                    _callback.Invoke(false);
                }
            }
#endif
            
        }
        
        public void Initialize()
        {
            _signal.Subscribe<StartAugmentedRealitySignal>(StartAR);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<StartAugmentedRealitySignal>(StartAR);
#if !UNITY_WEBGL
            VuforiaRuntime.Instance.UnregisterVuforiaInitErrorCallback(OnVuforiaInitializationError);
            VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
#endif
            DisposeAR();
        }
        
        private void SetObjectActive(bool value)
        {
            _load3DModelContainer.transform.Find("model").gameObject.SetActive(value);
            _load3DModelContainer.transform.Find("3DLabel").gameObject.SetActive(value);
        }

        private void VuforiaActivate()
        {
            _signal.Fire(new PopupOverlaySignal(false));
            SetObjectActive(true);

            _isSupportedAR = true;

            if (_isSupportedAR)
            {
                ActivateAR();
                _callback.Invoke(true);
            }
            else
            {
                DisposeAR();

                _callback.Invoke(false);

                _signal.Fire(new PopupOverlaySignal(true, UnsupportedDeviceBody, false, PopupOverlayType.TextBox,
                    UnsupportedDeviceTitle));
            }
        }
        
        private void ActivateAR()
        {
#if !UNITY_WEBGL
            if (!Application.isEditor)
            {
                //Start model Scale
                _load3DModelContainer.transform.localScale = GetScaleAR;

                _anchorBehaviour = _load3DModelContainer.gameObject.AddComponent<AnchorBehaviour>();
                _load3DModelContainer.gameObject.AddComponent<DefaultTrackableEventHandler>();

                _planeAR[0].planeFinder.GetComponent<ContentPositioningBehaviour>().AnchorStage = _anchorBehaviour;

                _planeAR[0].translationAr = _load3DModelContainer.GetComponent<TranslationAR>();
                _planeAR[0].rotationAr = _load3DModelContainer.GetComponent<RotationAR>();
                
                _planeAR[0].translationAr.camera = _vuforiaCamera;
                _planeAR[0].translationAr.enabled = true;
                _planeAR[0].translationAr.ActivateFloor(true);

                _planeAR[0].rotationAr.camera = _vuforiaCamera;
                _planeAR[0].rotationAr.enabled = true;
                
                _load3DModelContainer.GetComponent<ScaleAR>().enabled = true;
            }

            ContentViewModel.OnChangeLayerLanguage += Changelangueageonlayer;
            DataModel.VuforiaCamera = _vuforiaCamera;
            RotationAR.Interactable = true;
            TranslationAR.Interactable = true;
            ScaleAR.Interactable = true;

            UpdatePlaceModelLocalization();
            ShowPopupInfoAR();

            //Setup contentView 
            if (_contentViewModel.contentViewMobile)
            {
                _contentViewModel.contentViewMobile.colorPikerToggle.gameObject.SetActive(false);
                _contentViewModel.contentViewMobile.cameraOrigin.gameObject.SetActive(false);
                _contentViewModel.contentViewMobile.resetButton.gameObject.SetActive(false);
                
                _contentViewModel.contentViewMobile._placeModelAR.SetActive(true);
                _contentViewModel.contentViewMobile._placeModelAR.onValueChanged.RemoveAllListeners();
                _contentViewModel.contentViewMobile._placeModelAR.onValueChanged.AddListener((value) => PlaceModel(value, _planeAR[0]));
                if ( _planeAR[0].rotationAr)
                {
                    _planeAR[0].rotationAr.OnActive = null;
                    _planeAR[0].rotationAr.OnActive += (v) =>
                    {
                        _contentViewModel.contentViewMobile._placeModelAR.SetSelectedModelIcon(v);
                    };
                }
            }
            
            if (_contentViewModel.contentViewPC)
            {
                _contentViewModel.contentViewPC.zoomMinusButton.gameObject.SetActive(false);
                _contentViewModel.contentViewPC.zoomPlusButton.gameObject.SetActive(false);
                _contentViewModel.contentViewPC.arCameraToggle.gameObject.SetActive(false);
                _contentViewModel.contentViewPC.colorPikerToggle.gameObject.SetActive(false);
                _contentViewModel.contentViewPC.cameraOrigin.gameObject.SetActive(false);
                _contentViewModel.contentViewPC.resetButton.gameObject.SetActive(false);
                
                _contentViewModel.contentViewPC._placeModelAR.SetActive(true);
                _contentViewModel.contentViewPC._placeModelAR.onValueChanged.RemoveAllListeners();
                _contentViewModel.contentViewPC._placeModelAR.onValueChanged.AddListener((value) => PlaceModel(value, _planeAR[0]));

                if (_planeAR[0].rotationAr)
                {
                    _planeAR[0].rotationAr.OnActive = null;
                    _planeAR[0].rotationAr.OnActive += (v) =>
                    {
                        _contentViewModel.contentViewPC._placeModelAR.SetSelectedModelIcon(v);
                    };
                }
            }
#endif
        }

        private void ShowPopupInfoAR()
        {
            _signal.Fire(new PopupWarningARViewSignal(Info));

            void Info()
            {
                if (PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.ARGettingStartedInfo))
                    return;

                string msg = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.ARGettingStartedKey);
                string ok = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.OkKey);
                string dont = _contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.DontDisplayThisMessageAgainKey);

                _signal.Fire(new PopupInfoViewSignal(msg, ok, null, dont, ClosePopupInfoView, null));

                void ClosePopupInfoView(bool value)
                {
                    PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.ARGettingStartedInfo, value);
                }
            }
        }

        private void PlaceModel(bool value, (GameObject planeFinder, TranslationAR translationAr, RotationAR rotationAr) planeAR)
        {
#if !UNITY_WEBGL
            if(Application.isEditor)
                return;
            
            SetActivePlaneFinder( planeAR.planeFinder, value);

            if (!value)
            {
                planeAR.translationAr?.ResetPosition();
                planeAR.rotationAr?.ResetRotation();
                
                // Reset model position to AR ground marker
                planeAR.planeFinder.GetComponent<PlaneFinderBehaviour>().PerformHitTest(Vector2.zero);
            }
#endif
        }

        private void SetActivePlaneFinder(GameObject planeFinder, bool isActive)
        {
#if !UNITY_WEBGL
            if (planeFinder)
            {
                planeFinder.GetComponent<PlaneFinderBehaviour>().PlaneIndicator.SetActive(isActive);
            }
#endif
        }

        #region SecondAR
        
        public void AddSecondModel(int modelId)
        {
            _signal.Fire(new PopupOverlaySignal(true, GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));

            Debug.Log($">> AddSecondModel  modelId = {modelId}");

            _signal.Fire(new LoadAssetCommandSignal(modelId, (isLoaded, loadedID, model, msg) =>
            {
                if (isLoaded)
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    ModelLoaded(model);
                }
            }));

            void ModelLoaded(GameObject model)
            {
#if !UNITY_WEBGL
                CreateSecondARContainer(out var modelParent);

                CreateModel(model, "model", modelParent.transform);
                
                CreateLabelSecondModel(modelParent.transform);

                if (Application.isEditor) // Test AR <<<<<<<<<<<<<<<<<<<<<<
                    return;

                //Start model Scale
                modelParent.transform.localScale = GetScaleAR;
                
                if (!modelParent.gameObject.HasComponent(out AnchorBehaviour _anchorBehaviour2))
                {
                    _anchorBehaviour2 = modelParent.gameObject.AddComponent<AnchorBehaviour>();
                    modelParent.gameObject.AddComponent<DefaultTrackableEventHandler>();

                    if(_contentViewModel.contentViewMobile)
                        _planeAR[1].planeFinder = MonoBehaviour.Instantiate(_screenContainer.VuforiaPlaneFinder_2, _contentViewModel.contentViewMobile.cameraOrigin.parent);
                    
                    if(_contentViewModel.contentViewPC)
                        _planeAR[1].planeFinder = MonoBehaviour.Instantiate(_screenContainer.VuforiaPlaneFinder_2, _contentViewModel.contentViewPC.cameraOrigin.parent);
                    
                    _planeAR[1].planeFinder.transform.position = _planeAR[0].planeFinder.transform.position;
                    _planeAR[1].planeFinder.GetComponent<ContentPositioningBehaviour>().AnchorStage = _anchorBehaviour2;


                    _planeAR[1].translationAr = modelParent.GetComponent<TranslationAR>();
                    _planeAR[1].rotationAr = modelParent.GetComponent<RotationAR>();

                    _signal.Fire(new ShowDebugLogCommandSignal($"has _rotationAr2 {_planeAR[1].rotationAr} - enabled {_planeAR[1].rotationAr.enabled}"));

                    _planeAR[1].translationAr.camera = _vuforiaCamera;
                    _planeAR[1].translationAr.enabled = true;

                    _planeAR[1].rotationAr.camera = _vuforiaCamera;
                    _planeAR[1].rotationAr.enabled = true;

                    modelParent.GetComponent<ScaleAR>().enabled = true;

                    if (_contentViewModel.contentViewMobile)
                    {
                        _contentViewModel.contentViewMobile._placeModelAR_2.SetActive(true);
                        _contentViewModel.contentViewMobile._placeModelAR_2.onValueChanged.RemoveAllListeners();
                        _contentViewModel.contentViewMobile._placeModelAR_2.onValueChanged.AddListener((value) => PlaceModel(value, _planeAR[1]));
                        _contentViewModel.contentViewMobile._placeModelAR_2.SetValue(true, true);
                        _planeAR[1].rotationAr.OnActive = null;
                        _planeAR[1].rotationAr.OnActive += (v) => { _contentViewModel.contentViewMobile._placeModelAR_2.SetSelectedModelIcon(v);};
                    }

                    if (_contentViewModel.contentViewPC)
                    {
                        _contentViewModel.contentViewPC._placeModelAR_2.SetActive(true);
                        _contentViewModel.contentViewPC._placeModelAR_2.onValueChanged.RemoveAllListeners();
                        _contentViewModel.contentViewPC._placeModelAR_2.onValueChanged.AddListener((value) => PlaceModel(value, _planeAR[1]));
                        _contentViewModel.contentViewPC._placeModelAR_2.SetValue(true, true);
                        _planeAR[1].rotationAr.OnActive = null;
                        _planeAR[1].rotationAr.OnActive += (v) => { _contentViewModel.contentViewPC._placeModelAR_2.SetSelectedModelIcon(v);};
                    }
                }
#endif
            }
        }

        private const string nameSecondAR = "SecondAR";
        private void CreateSecondARContainer(out GameObject arContainer)
        {
            TranslationAR tar = FindComponentExtension.GetInScene<TranslationAR>(nameSecondAR, SceneNameConstants.Module3DModel);

            if (tar == null)
            {
                arContainer = new GameObject(nameSecondAR);
                SceneManager.MoveGameObjectToScene(arContainer, SceneManager.GetSceneByName(SceneNameConstants.Module3DModel));
                arContainer.AddComponent<TranslationAR>();
                arContainer.AddComponent<RotationAR>();
                arContainer.AddComponent<ScaleAR>();
                arContainer.AddComponent<ControllerAR>().GetReference();
                _load3DModelContainer.gameObject.AddComponent<ControllerAR>().GetReference();
            }
            else
            {
                arContainer = tar.gameObject;

                foreach (Transform t in arContainer.transform)
                {
                    MonoBehaviour.Destroy(t.gameObject);
                }
            }

            arContainer.transform.position = Vector3.zero;
            arContainer.transform.rotation = Quaternion.identity;
            arContainer.transform.localScale = Vector3.one;
        }

        private bool isWrappedFirstModel;
        private List<Toggle> secondModelToggle = new List<Toggle>();
        private void CreateLabelSecondModel(Transform parent)
        {
            if (secondModelToggle.Count > 0)
            {
                secondModelToggle.ForEach(t => MonoBehaviour.Destroy(t.gameObject));
            }
            secondModelToggle.Clear();

            if (DeviceInfo.IsMobile())
                MobileLabelSecondModel(parent);
            else
                PCLabelSecondModel(parent);
        }

        private void Changelangueageonlayer((int layer, string cultureCode) valueTuple)
        {
            if(labelListSecondAsset)
                _asyncProcessor.Wait(0, () => labelListSecondAsset.SortToggleItemByText());

            UpdatePlaceModelLocalization();
        }

        private void UpdatePlaceModelLocalization()
        {
            if (_contentViewModel.contentViewMobile)
            {
                _contentViewModel.contentViewMobile._placeModelAR.SetLocalization(
                    _contentViewModel.GetCurrentSystemTranslations(_contentViewModel.contentViewMobile._placeModelAR
                        .GetKeyTrue()),
                    _contentViewModel.GetCurrentSystemTranslations(_contentViewModel.contentViewMobile._placeModelAR
                        .GetKeyFalse())
                );

                _contentViewModel.contentViewMobile._placeModelAR_2.SetLocalization(
                    _contentViewModel.GetCurrentSystemTranslations(_contentViewModel.contentViewMobile._placeModelAR_2
                        .GetKeyTrue()),
                    _contentViewModel.GetCurrentSystemTranslations(_contentViewModel.contentViewMobile._placeModelAR_2
                        .GetKeyFalse())
                );
            }
        }

        private DropdownToggle labelListSecondAsset;
        private void PCLabelSecondModel(Transform parent)
        {
            if (labelListSecondAsset == null)
            {
                _contentViewModel.contentViewPC.labelsListDropdownToggle.DropdownListToggle.isOn = false;

                _contentViewModel.contentViewPC.labelsListDropdownToggle.DropdownListToggle.transform
                    .GetComponentByName<TextMeshProUGUI>("Text_ico").text = "1";
                
                labelListSecondAsset = MonoBehaviour
                    .Instantiate(_contentViewModel.contentViewPC.labelsListDropdownToggle.gameObject,
                        _contentViewModel.contentViewPC.labelsListDropdownToggle.transform.parent, false)
                    .GetComponent<DropdownToggle>();
                
                labelListSecondAsset.transform.GetComponentByName<TextMeshProUGUI>("Text_ico").text = "2";
                
                labelListSecondAsset.transform.SetSiblingIndex(_contentViewModel.contentViewPC.labelsListDropdownToggle.transform.GetSiblingIndex()+1);
                
                _contentViewModel.AddTooltipToTranslator(labelListSecondAsset.gameObject.GetAllComponentsInChildren<TooltipEvents>(true));
                _contentViewModel.AddSystemTextToTranslator((LocalizationConstants.LabelsKey, labelListSecondAsset.DropdownLabel));
                _contentViewModel.AddSystemTextToTranslator((LocalizationConstants.SelectAllKey, labelListSecondAsset.Template.GetComponentInChildren<TextMeshProUGUI>()));
            }
            
            if (!_contentModel.HasAssetLabels(_contentViewModel.secondAssetID))
            {
                labelListSecondAsset.gameObject.SetActive(false);
                return;
            }
            
            labelListSecondAsset.ClearDropdownToggle();
            labelListSecondAsset.ClearTemplateListeners();
            
            labelListSecondAsset.gameObject.SetActive(true);

            GameObject labelParent = new GameObject("3DLabel");

            SceneManager.MoveGameObjectToScene(labelParent, SceneManager.GetSceneByName(SceneNameConstants.Module3DModel));

            labelParent.transform.SetParent(parent);
            labelParent.transform.localScale = Vector3.one;


            var assetLabels = _contentModel.GetAssetById(_contentViewModel.secondAssetID).AssetDetail.AssetContentPlatform.assetLabel;
            foreach (var v in assetLabels)
            {
                ColorUtility.TryParseHtmlString(v.highlightColor, out Color newCol);

                var itemKey = v.labelLocal.First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;

                LabelData ld = _labelFactory.Create(itemKey, newCol);

                Vector3 sc = ld.transform.localScale;
                ld.transform.SetParent(labelParent.transform);

                ld.transform.localScale = sc;

                ld.ID = _contentViewModel.secondAssetID;
                ld.LabelId = ld.ID + "_" + v.labelId;
                ld.LabelLocalNames = v.labelLocal;
                ld.SetModelPartName(itemKey);

                ld.transform.position = new Vector3(v.position.x, v.position.y, v.position.z);
                ld.transform.rotation = Quaternion.Euler(v.rotation.x, v.rotation.y, v.rotation.z);

                var localizedName = v.labelLocal.First(locale => locale.Culture.Equals(_contentViewModel.CurrentContentViewCultureCode)).Name;

                Toggle tgl = labelListSecondAsset.CreateLabelToggle($"{localizedName}", ld.gameObject);
                tgl.gameObject.name = itemKey;
                
                _contentViewModel.AddLabelTextToTranslator((_contentViewModel.secondAssetID, v.labelId, tgl.GetComponentInChildren<TextMeshProUGUI>(), ld));

                secondModelToggle.Add(tgl);
                ObjectHighlighter oh = parent.Find($"model/{itemKey}")?.gameObject.AddComponent<ObjectHighlighter>();
                if (oh)
                {
                    oh.ID = _contentViewModel.secondAssetID;
                    oh.SetToggle(tgl);
                    oh.SetColor(newCol);
                    tgl.onValueChanged.AddListener((isOn) => oh.SetHighlightColor(isOn));
                }
            }

            labelParent.transform.localRotation = Quaternion.identity;
        }

        private void MobileLabelSecondModel(Transform parent)
        {
            //Toggle template = _contentViewModel.contentViewMobile.labelsListDropdownToggle.Template.GetComponent<Toggle>();
            if (!isWrappedFirstModel)
            {
                string accordionName =  _contentModel.GetAssetLocalizedName(_contentViewModel.mainAssetID)[_contentViewModel.CurrentContentViewCultureCode];

                Toggle accordionTgl1st = _contentViewModel.contentViewMobile.labelsListDropdownToggle.CreateLabelToggle($"> {accordionName}", null);
                accordionTgl1st.gameObject.name = accordionName;
                accordionTgl1st.isOn = true;
                accordionTgl1st.onValueChanged.RemoveAllListeners();
                accordionTgl1st.transform.SetSiblingIndex(0);

                accordionTgl1st.gameObject.GetAllComponentsInChildren<Image>().ForEach(img =>
                {
                    img.color = new Color32(53, 153, 153, 255);
                });

                _contentViewModel.contentViewMobile.labelsListDropdownToggle.ToggleGroupContainer.gameObject
                    .GetAllComponentsInChildren<Toggle>().ForEach(cTgl =>
                    {
                        if (cTgl.gameObject.activeSelf && !accordionTgl1st.Equals(cTgl))
                        {
                            accordionTgl1st.onValueChanged.AddListener(cTgl.gameObject.SetActive);
                        }
                    });

                accordionTgl1st.SetValue(false, true);
                isWrappedFirstModel = true;
            }

            if (!_contentModel.HasAssetLabels(_contentViewModel.secondAssetID))
            {
                return;
            }

            GameObject labelParent = new GameObject("3DLabel");
            labelParent.transform.parent = parent;
            SetActiveScreenLabel(false, _contentViewModel.contentViewMobile);
            _contentViewModel.contentViewMobile.labelsListDropdownToggle.DropdownListToggle.gameObject.SetActive(true);
            string accordion2Name = _contentModel.GetAssetLocalizedName(_contentViewModel.secondAssetID)[_contentViewModel.CurrentContentViewCultureCode];

            Toggle accordionTgl2nd = _contentViewModel.contentViewMobile.labelsListDropdownToggle.CreateLabelToggle($"> {accordion2Name}", null);
            accordionTgl2nd.gameObject.name = accordion2Name;
            accordionTgl2nd.isOn = true;
            accordionTgl2nd.onValueChanged.RemoveAllListeners();
            secondModelToggle.Add(accordionTgl2nd);
            accordionTgl2nd.gameObject.GetAllComponentsInChildren<Image>().ForEach(img => { img.color = new Color32(53, 153, 153, 255); });

            var assetLabels = _contentModel.GetAssetById(_contentViewModel.secondAssetID).AssetDetail.AssetContentPlatform
                .assetLabel;
            int i = assetLabels.Length;
            foreach (var v in assetLabels)
            {
                i++;
                ColorUtility.TryParseHtmlString(v.highlightColor, out Color newCol);

                var itemKey = v.labelLocal.First(locale => locale.Culture.Equals(_localizationModel.FallbackCultureCode)).Name;

                LabelData ld = _labelFactory.Create(i.ToString(), newCol);
                ld.transform.parent = labelParent.transform;
                ld.ID = _contentViewModel.secondAssetID;
                ld.LabelId = ld.ID + "_" + v.labelId;
                ld.LabelLocalNames = v.labelLocal;
                ld.SetModelPartName(itemKey);

                ld.transform.position = new Vector3(v.position.x, v.position.y, v.position.z);
                ld.transform.rotation = Quaternion.Euler(v.rotation.x, v.rotation.y, v.rotation.z);

                var localizedName = v.labelLocal.First(locale => locale.Culture.Equals(_contentViewModel.CurrentContentViewCultureCode)).Name;

                Toggle tgl = _contentViewModel.contentViewMobile.labelsListDropdownToggle.CreateLabelToggle($"{i}. {localizedName}", ld.gameObject);
                tgl.gameObject.name = itemKey;
                tgl.onValueChanged.AddListener((value) => SetActiveScreenLabel(value, _contentViewModel.contentViewMobile, tgl, itemKey));
                accordionTgl2nd.onValueChanged.AddListener(tgl.gameObject.SetActive);

                switch (ld)
                {
                    case LabelDataMobile ldm:
                        ldm.myToggleText = tgl.GetComponentInChildren<TextMeshProUGUI>(true);
                        break;
                }
                
                secondModelToggle.Add(tgl);
                ObjectHighlighter oh = parent.Find($"model/{itemKey}")?.gameObject.AddComponent<ObjectHighlighter>();
                if (oh)
                {
                    oh.ID = _contentViewModel.secondAssetID;
                    oh.SetToggle(tgl);
                    oh.SetColor(newCol);
                    tgl.onValueChanged.AddListener((isOn) => oh.SetHighlightColor(isOn));
                }
            }

            accordionTgl2nd.SetValue(false, true);
            labelParent.transform.localRotation = Quaternion.identity;
        }

        #endregion

        private void DisposeAR()
        {
            
            if(!_isActiveAR)
                return;
            
            if( _planeAR[0].planeFinder != null)
                MonoBehaviour.Destroy( _planeAR[0].planeFinder);
#if !UNITY_WEBGL
            if (VuforiaBehaviour.Instance != null)
            {
                VuforiaBehaviour.Instance.enabled = false;
                MonoBehaviour.Destroy(VuforiaBehaviour.Instance.gameObject);
            }
            
            if (_planeAR[0].translationAr != null)
            {
                _planeAR[0].translationAr.ActivateFloor(false);
            }
#endif
        }
        
        public bool IsRunAR()
        {
            return _isSupportedAR && _isActiveAR;
        }

        #region VuforiaEvents
#if !UNITY_WEBGL
        public void OnVuforiaInitializationError(VuforiaUnity.InitError initError)
        {
            if (initError != VuforiaUnity.InitError.INIT_SUCCESS)
            {
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"!!! ERROR !!! => {initError.ToString()}"));
                _signal.Fire(new PopupOverlaySignal(true, initError.ToString(), false, PopupOverlayType.TextBox, "Error"));
            }
        }
#endif

        void OnVuforiaStarted()
        {
            if(Application.isEditor) // Test AR <<<<<<<<<<<<<
                return;
#if !UNITY_WEBGL
            // Check trackers to see if started and start if necessary
            _positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
            _smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
            
            if (_positionalDeviceTracker != null && _smartTerrain != null)
            {
                if (!_positionalDeviceTracker.IsActive)
                    _positionalDeviceTracker.Start();
                if (_positionalDeviceTracker.IsActive && !_smartTerrain.IsActive)
                    _smartTerrain.Start();

                _isSupportedAR = true;
            }
            else
            {
                if (_positionalDeviceTracker == null)
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("PositionalDeviceTracker returned null. GroundPlane not supported on this device."));
                if (_smartTerrain == null)
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("SmartTerrain returned null. GroundPlane not supported on this device."));
                
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"!!! ERROR !!! => {UnsupportedDeviceTitle}"));
                
                _isSupportedAR = false;
            }
            #endif
            _signal.Fire(new ShowDebugLogCommandSignal($"AR Check: _isSupportedAR = {_isSupportedAR}"));
        }

        #endregion
    }
}