using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TDL.Commands;
using TDL.Constants;
using TDL.Models;
using TDL.Modules.Model3D;
using TDL.Server;
using TDL.Signals;
using TMPro;
using UnityEngine;
using Zenject;

public class ContentLessonModeViewMediator  : ContentViewMediatorBase, IInitializable, IDisposable, ITickable, IMediator
{
    [Inject] private MultiView.Factory  _multiViewFactory;
    [Inject] private Load3DModelContainer _load3DModelContainer;
    [Inject] private AccessibilityModel _accessibilityModel;
    
    private ContentHomeView _viewHome => _contentViewModel.contentHomeView;
    private ContentViewPC _viewPC => _contentViewModel.contentViewPC;
    private ContentHomeLectureModeView _lectureModePanel => _viewHome.contenentHomeLecturePanel;
    private bool _isOnTTS => _accessibilityModel.IsActiveTextToAudio;
    
    private MultiView _multiView;
    private List<(bool isLabel, Assetlabel label, Asset asset)> _assetlabels = new List<(bool isLabel, Assetlabel label, Asset asset)>();
    private int _currentPart = -1;
    private bool _isInitPlaying = false;
    private bool _isPlayTextAudioLoaded = false;
    private string _playText;
    private bool _isPlayFirstTime;
    private AudioSource _audioSource => _viewHome.audioSource;
     
    public void Initialize()
    {
        InitializeModuleAction += InitializeModule;
        ShowContentHomeScreenAction += ShowContentHomeScreen;
            
        _viewHome.lessonButton.onClick.AddListener(() => InteractiveMode(true));

        
        _lectureModePanel.nextButton.onClick.AddListener(SelectNextPart);
        _lectureModePanel.previousButton.onClick.AddListener(SelectPrevPart);
        
        _lectureModePanel.playButton.onClick.AddListener(() => Play(true));
        _lectureModePanel.pauseButton.onClick.AddListener(() => Play(false));

        _lectureModePanel.homeButton.onClick.AddListener(() => InteractiveMode(false));
    }
    
    private void InitializeModule(int id)
    {
        UpdateModelParts();
        UpdateLocalization();
    }

    private void ShowContentHomeScreen(bool isActive)
    {
        UpdateModelParts();
        UpdateLocalization();
    }

    private void UpdateLocalization()
    {
        _lectureModePanel.previousButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GetCurrentSystemTranslations(LocalizationConstants.Content3DModelPreviousKey);
        _lectureModePanel.nextButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GetCurrentSystemTranslations(LocalizationConstants.Content3DModelNextKey);
        
        _lectureModePanel.playButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GetCurrentSystemTranslations(LocalizationConstants.PlayKey);
        _lectureModePanel.pauseButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GetCurrentSystemTranslations(LocalizationConstants.PauseKey);
        _lectureModePanel.homeButton.GetComponentInChildren<TextMeshProUGUI>().text =
            GetCurrentSystemTranslations(LocalizationConstants.Content3DModelHomeKey);
    }

    private void UpdateModelParts()
    {
        _assetlabels.Clear();
        ClientAssetModel clientAssetModel = _contentModel.GetAssetById(_contentViewModel.mainAssetID);

        // Get all label descriptions
        clientAssetModel?.AssetDetail?.AssetContentPlatform?.assetLabel?.ToList().ForEach(
            al => { _assetlabels.Add((true, al, null)); });

        _assetlabels.Sort((a, b) => a.label.partOrder.CompareTo(b.label.partOrder));

        _assetlabels.RemoveAll(rm =>
        {
            return string.IsNullOrEmpty(rm.label.labelLocal.ToList()
                .Find(f => f.Culture == _contentViewModel.CurrentContentViewCultureCode).DescriptionUrl);
        });

        bool hasLabelDescription = _assetlabels.FindAll(l => l.label.partOrder > 0).Count > 0 && _assetlabels.Count > 0;
        
         // Get asset description
         var desc = _contentModel?.GetCurrentAssetLocalDesc(_contentViewModel.mainAssetID, true);

        bool hasAssetDescription = !string.IsNullOrEmpty(desc?
            .Find(ld => ld.Culture == _contentViewModel.CurrentContentViewCultureCode)?.DescriptionUrl);

        if (hasAssetDescription)
        {
            clientAssetModel.Asset.LocalizedStudentDescription = desc.ToArray();
            _assetlabels.Insert(0, (false, null, clientAssetModel.Asset));
        }
        
        // Show Lesson mode
        bool hasAssetsToShow = hasLabelDescription || hasAssetDescription;
        _viewHome.lessonButton.gameObject.SetActive(clientAssetModel.HasLessonMode && hasAssetsToShow);
    }

    private void InteractiveMode(bool isOn)
    {
        _viewHome.homePanel.SetActive(!isOn);
        RayCastBlock(!isOn);
        InitMode(isOn);
    }
        
    private void RayCastBlock(bool isBlock)
    {
        _viewHome.rayCastBlock.raycastTarget = isBlock;
    }
    private void InitMode(bool isOn)
    {
        _lectureModePanel.SetActive(isOn);
        _currentPart = -1;
        _isInitPlaying = false;
        _isPlayFirstTime = true;

        Play(false);
        ChangeCameraView(isOn);

        if (isOn)
            SelectNextPart();
    }

    private void Play(bool isPlay)
    {
        PlayButtonState(isPlay);

        if (isPlay)
        {
            if (string.IsNullOrEmpty(_playText))
            {
                _audioSource.Play();
            }
            else
            {
                if (_isPlayTextAudioLoaded)
                    _audioSource.Play();
                else
                    LoadAndPlaySound(_contentViewModel.CurrentContentViewCultureCode, _playText);
            }
        }
        else
        {
            _audioSource.Pause();
        }
    }

    private void LoadAndPlaySound(string CultureCode, string text)
    {
        _audioSource.Stop();
        _isInitPlaying = true;
        _signal.Fire(new LoadAndPlayDescriptionCommandSignal(_audioSource, CultureCode, text, true));
    }

    private void ShowLoadingPanel(bool isActive)
    {
        _signal.Fire(new PopupOverlaySignal(isActive, GetCurrentSystemTranslations(LocalizationConstants.LoadingKey)));
    }
    
    private void PlayButtonState(bool isPlay)
    {
        _lectureModePanel.playButton.gameObject.SetActive(!isPlay);
        _lectureModePanel.pauseButton.gameObject.SetActive(isPlay);
    }
    
    private void SetPlayButtonInteractable(bool isInteractable)
    {
        _lectureModePanel.playButton.interactable = isInteractable;
        _lectureModePanel.pauseButton.interactable = isInteractable;
    }

    private void ChangeCameraView(bool isOn)
    {
        if (isOn)
        {
            _viewPC.model3DCamera.DORect(ViewportRectPresets.TopLeft, 0.5f).SetEase(Ease.OutQuad);
            
            _multiView = _multiViewFactory.Create(_viewHome.gameObject.layer + 1, MultiViewType.Multipart);
            
            if (_viewPC.backgroundRawImage.texture)
            {
                _multiView.BackgroundRawImage.gameObject.SetActive(true);
                _multiView.BackgroundRawImage.texture = _viewPC.backgroundRawImage.texture;
            }
            
            _multiView.MultimodelView.SmoothOrbitCam.interactable = true;
            _multiView.Frame.gameObject.SetActive(false);
            _multiView.MultimodelView.SetActiveUI(false);
            _multiView.MultipartView.SetActiveUI(false);
            SetupModelControlButtons();
        }
        else
        {
            _viewPC.model3DCamera.DORect(ViewportRectPresets.FullScreen, 0.5f).SetEase(Ease.OutQuad).onComplete += () =>
            {
                _multiView.gameObject.SelfDestroy();
            };
        }
    }

    private void SetupModelControlButtons()
    {
        _multiView.MultipartView.SetActiveUI(true);
        _multiView.MultipartView.CloseScreen.gameObject.SetActive(false);
        _multiView.MultipartView.ScreenName.gameObject.SetActive(false);
        
        _multiView.MultipartView.gameObject.GetAllComponentsInChildren<TooltipEvents>().ForEach(t =>
        {
            var keyTranslation = t.GetKey();

            if (!string.IsNullOrEmpty(keyTranslation))
            {
                t.SetHint(_localizationModel.GetSystemTranslations(_contentViewModel.CurrentContentViewCultureCode, keyTranslation));
            }
        });

        // Reset
        _multiView.MultipartView.ResetButton.onClick.AddListener(() =>
        {
            CameraResetBase(_multiView.MultipartView.SmoothOrbitCam, true, false, IsAsset360Model(_contentViewModel.mainAssetID));
        });
        
        // Change background color
        _multiView.MultipartView.ColorPicker.ActivationChanged.AddListener(value =>
        {
            _multiView.BackgroundRawImage.gameObject.SetActive(!value);
        });
        
        _multiView.MultipartView.ColorPicker.onValueChanged.AddListener(value =>
        {
            _multiView.MultipartView.RenderCamera.backgroundColor = value;
        });
            
        //Zoom Plus
        _multiView.MultipartView.ZoomPlusButton.onClick.AddListener(
            () => _multiView.MultipartView.SmoothOrbitCam.Zoom(0.5f));
            
        //Zoom Minus
        _multiView.MultipartView.ZoomMinusButton.onClick.AddListener(
            () => _multiView.MultipartView.SmoothOrbitCam.Zoom(-0.5f));
    }

    private void SelectNextPart()
    {
        _currentPart++;

        if (_currentPart > _assetlabels.Count - 1)
        {
            _currentPart = _assetlabels.Count - 1;
        }
        else
        {
            SelectPart();
        }
        
        UpdateNextPrevButtons();
    }
    
    private void SelectPrevPart()
    {
        _currentPart--;
        
        if (_currentPart < 0)
        {
            _currentPart = 0;
        }
        else
        {
            SelectPart();
        }

        UpdateNextPrevButtons();
    }

    private void UpdateNextPrevButtons()
    {
        _lectureModePanel.nextButton.interactable = _currentPart != _assetlabels.Count - 1;
        _lectureModePanel.previousButton.interactable = _currentPart != 0;
    }

    private void SelectPart()
    {
        _multiView.RenderLayer.DestroyChildren();
        string urlDescription;
        string urlAudioFile;

        if (_assetlabels[_currentPart].isLabel)
        {
            CreateLabel();
        }
        else
        {
            CreateAsset();
        }

        void CreateAsset()
        {
            _lectureModePanel.labelName.text =  _assetlabels[_currentPart].asset.AssetLocal.Find(local => local.Culture == _contentViewModel.CurrentContentViewCultureCode).Name;
            
            urlAudioFile = _assetlabels[_currentPart].asset.LocalizedStudentDescription.ToList()
                .Find(ld => ld.Culture == _contentViewModel.CurrentContentViewCultureCode).AudioFileUrl;

            urlDescription = _assetlabels[_currentPart].asset.LocalizedStudentDescription.ToList()
                .Find(ld => ld.Culture == _contentViewModel.CurrentContentViewCultureCode).DescriptionUrl;

            _multiView.RenderLayer.transform.rotation = _load3DModelContainer.transform.rotation;
            GameObject model = _load3DModelContainer.transform.Find("model").gameObject;
            
            GameObject newGO = MonoBehaviour.Instantiate(model, _multiView.RenderLayer.transform, false);
            newGO.transform.localRotation = Quaternion.identity;
            
            newGO.transform.SetLayer(_multiView.RenderLayer.layer);

            newGO.GetAllComponentsInChildren<ObjectHighlighter>().ForEach(oh =>
            {
                oh.OffHighlightMaterial(true);
                oh.interactable = false;
                oh.SelfDestroy();
            });
        }

        void CreateLabel()
        {
            _lectureModePanel.labelName.text = GetAllTranslationsForItem(
                _assetlabels[_currentPart].label.labelLocal.ConvertToLocalName(),
                _contentViewModel.CurrentContentViewCultureCode);
            
            urlAudioFile = _assetlabels[_currentPart].label.labelLocal.ToList()
                .Find(ld => ld.Culture == _contentViewModel.CurrentContentViewCultureCode).AudioFileUrl;

            urlDescription = _assetlabels[_currentPart].label.labelLocal.ToList().Find(f => f.Culture == _contentViewModel.CurrentContentViewCultureCode).DescriptionUrl;

            string partName = GetAllTranslationsForItem(
                _assetlabels[_currentPart].label.labelLocal.ConvertToLocalName(),
                LocalizationConstants.EnUSLanguage);

            ObjectHighlighter oh = _multiView.gameObject.GetInScene<ObjectHighlighter>(partName);

            _multiView.RenderLayer.transform.rotation = _load3DModelContainer.transform.rotation;

            ObjectHighlighter newOH = MonoBehaviour.Instantiate(oh.gameObject, _multiView.RenderLayer.transform, false)
                .GetComponent<ObjectHighlighter>();
            newOH.transform.localRotation = Quaternion.identity;

            // Set Pivot Point to Center Of Mass
            SetPivotPoint(newOH.gameObject, _contentViewModel.mainAssetID);

            newOH.transform.SetLayer(_multiView.RenderLayer.layer);
            
            _asyncProcessor.Wait(0f, () =>
            {
                newOH.OffHighlightMaterial(true);
                newOH.interactable = false;
            });
        }
        
        _asyncProcessor.Wait(0, () =>
        {
            CameraResetBase(_multiView.MultipartView.SmoothOrbitCam, true, false, IsAsset360Model(_contentViewModel.mainAssetID));
        });

        _lectureModePanel.description.text = string.Empty;
        _playText = string.Empty;
        _audioSource.Stop();
        _audioSource.clip = null;
        bool isLoadingDone = false;
        
        if (!string.IsNullOrEmpty(urlAudioFile))
        {
            ShowLoadingPanel(true);
            _signal.Fire(new GetAudioFileCommandSignal(urlAudioFile, (clip) =>
            {
                if (isLoadingDone)
                    ShowLoadingPanel(false);

                isLoadingDone = true;
                _audioSource.clip = clip;
                if (_isPlayFirstTime)
                {
                    _isPlayFirstTime = false;
                    Play(true);
                }
            }));
        }
        else
        {
            isLoadingDone = true;
        }
        
        if (!string.IsNullOrEmpty(urlDescription))
        {
            ShowLoadingPanel(true);
            _signal.Fire(new GetLabelDescriptionCommandSignal(urlDescription, (text) =>
            {
                if (isLoadingDone)
                    ShowLoadingPanel(false);

                isLoadingDone = true;
                _lectureModePanel.description.text = text;

                if(string.IsNullOrEmpty(urlAudioFile) && LocalizationConstants.EnUSLanguage.Equals(_localizationModel.CurrentLanguageCultureCode))
                {
                    _isPlayTextAudioLoaded = false;
                    _playText = text;

                    if (_isPlayFirstTime)
                    {
                        _isPlayFirstTime = false;
                        Play(true);
                    }
                }
            }));
        }
        else
        {
            isLoadingDone = true;
        }

        if (LocalizationConstants.EnUSLanguage.Equals(_localizationModel.CurrentLanguageCultureCode))
        {
            SetPlayButtonInteractable(true);
        }
        else
        {
            if (string.IsNullOrEmpty(urlAudioFile))
            {
                _isPlayFirstTime = false;
                SetPlayButtonInteractable(false);
            }
            else
            {
                SetPlayButtonInteractable(true);
            }
        }
    }
    
    
    public void Tick()
    {
        if (_lectureModePanel.pauseButton.gameObject.activeSelf && _lectureModePanel.gameObject.activeSelf && _audioSource && _audioSource.clip)
        {
            if(_audioSource.isPlaying)
            {
                if (_isInitPlaying && _audioSource.clip.length > 1.5f)
                {
                    _isInitPlaying = false;
                }
            }
            else 
            {
                if (!_isInitPlaying && _audioSource.clip.length > 1.5f)
                {
                    PlayButtonState(false);
                }
            }
        }
    }
    
    public void Dispose()
    {
        InitializeModuleAction -= InitializeModule;
        ShowContentHomeScreenAction -= ShowContentHomeScreen;
    }

    public void OnViewEnable()
    {
    }

    public void OnViewDisable()
    {
    }
}