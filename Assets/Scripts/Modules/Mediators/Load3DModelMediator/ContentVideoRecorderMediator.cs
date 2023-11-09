using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TDL.Constants;
using UnityEngine;
using Zenject;
using TDL.Models;
using TDL.Modules.Model3D.View;
using TDL.Signals;
using TDL.Views;
using UnityEngine.Networking;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.Apple.ReplayKit;
#endif

public class ContentVideoRecorderMediator : IInitializable, IMediator
{
    [Inject] private AsyncProcessorService _asyncProcessor;
    [Inject] private readonly SignalBus _signal;
    [Inject] private ScreenRecorderManager _screenRecorderManager;
    [Inject] private FileManager _fileManager;
    [Inject] private LocalizationModel _localization;
    [Inject] private VideoView.Factory _factory;

    private VideoView _view;
    private string _cultureCode;
    private int _assetID;
    private string _videoPath;
    
    private const string IconName = "Image_ico"; 

    public void Initialize()
    {
        _view = _factory.Create();
        _view.gameObject.SetActive(false);

        _view.closeButton.onClick.AddListener(CloseVideo);
        _view.saveButton.onClick.AddListener(SaveVideo);
        _view.sendButton.onClick.AddListener(SendVideo);
        
        _signal.Subscribe<StartVideoRecordingSignal>(StartRecordingSignal);
        _signal.Subscribe<VideoRecordingStateSignal>(OnChangeRecordingState);

        //_screenRecorderManager.AddStateListener(OnChangeRecordingState);
        _screenRecorderManager.AddRecordedVideoListener(VideoRecorded);

        _asyncProcessor.OnApplicationQuitAction += DeleteVideoFile;
    }

    private void StartRecordingSignal(StartVideoRecordingSignal signal)
    {
        _cultureCode = signal.CultureCore == "" ? _localization.CurrentLanguageCultureCode : signal.CultureCore;
        _assetID = signal.AssetID;
        
#if UNITY_WSA && !UNITY_EDITOR
        bool canRecord = UWP.VideoRecorder.IsCanRecord();

        if (!canRecord && signal.IsRecording)
        {
            HelpCantRecord();
            signal.ToggleVideoRecording.SetValue(false, false);
            return;
        }

#elif UNITY_IOS

        if (signal.IsRecording)
        {
            ReplayKit.StartRecording(true);
        }
        else
        {
            _signal.Fire(new VideoRecordingStateSignal(RecordingState.StopRecording));
            ReplayKit.StopRecording();
        }
#else    
        if (PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.HelpVideoRecordingToggle))
        {
            Record();
        }
        else
        {
            if (signal.IsRecording)
            {
                Help();
            }
            else
            {
                Record();
            }
        }
#endif

        void Record()
        {
            OnRecorder(signal.IsRecording);
        }
        
        void Help()
        {
            string msg =  _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.HelpVideoRecordingMessageKey);
            string ok =  _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.OkKey);
            string help =  _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.HelpKey);
            string dont =  _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.DontDisplayThisMessageAgainKey);

            _signal.Fire(new PopupInfoViewSignal(msg, ok, help, dont, OkCallback, HelpCallback));

            void OkCallback(bool dontShowAgain)
            {
                if(dontShowAgain)
                    PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.HelpVideoRecordingToggle, true);
            
                _asyncProcessor.Wait(0, Record);
            }
            
            void HelpCallback()
            {
                _signal.Fire(new OpenUrlCommandSignal(ServerConstants.HelpVideoRecordingUrl));
            }
        }
        
        void HelpCantRecord()
        {
            string error = _screenRecorderManager.GetUWPStatusError();
            string msg =  _localization.GetSystemTranslations(_cultureCode, error);
            string ok =  _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.OkKey);
            string help =  _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.HelpKey);

            _signal.Fire(new PopupInfoViewSignal(msg, ok, help, null, null, HelpCallback));
            
            void HelpCallback()
            {
                _signal.Fire(new OpenUrlCommandSignal(ServerConstants.HelpVideoCantRecordingUrl + error));
            }
        }
    }
    
    public void OnRecorder(bool isRecording) 
    {
        if (isRecording && _screenRecorderManager.GetCurrentState() != RecordingState.StartRecording )
        {
            StartRecord();
        }
        else
        {
            StopRecord();
        }
    }
    
    private void StartRecord()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _asyncProcessor.Wait(0, () 
            => _screenRecorderManager.StartCoroutine(_screenRecorderManager.RequestMicrophoneAccess()));
        Debug.Log("Called from coroutine");
#else
        _asyncProcessor.Wait(0, () 
            => _screenRecorderManager.StartRecording());
#endif
    }
    
    private void StopRecord()
    {
        _screenRecorderManager.StopRecording();
    }
    
    public void StopRecorder() 
    {
        if (_screenRecorderManager.GetCurrentState() == RecordingState.StartRecording)
            _screenRecorderManager.StopRecording();
    }
    
    private void VideoRecorded(string path)
    {
        _videoPath = path;
       
        UpdateVideoViewLocalization();

        _view.gameObject.SetActive(true);
        _view.panelVideo.SetActive(true);
        _view.transform.SetAsLastSibling();
        _view.videoPlayer.ClearTexture();
        
        _asyncProcessor.Wait(1f, () =>
        {
            _view.videoPlayer.InitializeModule(path);
        });
        
        SetButtonInteractable(_view.saveButton, true);
        SetButtonInteractable(_view.sendButton, true);
    }

    private void OnChangeRecordingState(VideoRecordingStateSignal signal) 
    {
        _signal.Fire(new PopupOverlaySignal(signal.State == RecordingState.StartRecording, type: PopupOverlayType.RecordingFrame));
    }

    private void SaveUserContentServerResponse(SaveUserContentServerResponseSignal signal)
    {
        if (signal.IsUploaded == false)
        {
            _signal.Fire(new PopupOverlaySignal(true, $"{signal.Progress} %"));
            return;
        }
        
        _signal.TryUnsubscribe<SaveUserContentServerResponseSignal>(SaveUserContentServerResponse);

        _signal.Fire(new PopupOverlaySignal(false));

        if (signal.Response.Success)
        {
            SetButtonInteractable(_view.sendButton, false);
            _signal.Fire(new PopupOverlaySignal(true, _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.ServerFileSavedKey), type: PopupOverlayType.MessageBox));
        }
        else
        {
            SetButtonInteractable(_view.sendButton, true);
            _signal.Fire(new PopupOverlaySignal(true, signal.Response.ErrorMessage, type: PopupOverlayType.TextBox));
            Debug.Log($"{signal.Response.ErrorMessage}");
        }
    }
    
    private void CloseVideo()
    {
        _signal.Fire(new FocusKeyboardNavigationSignal(_view.canvasGroup, false));

        _view.videoPlayer.Play(false);
        _view.gameObject.SetActive(false);
        _view.panelVideo.SetActive(false);
        DeleteVideoFile();
    }

    private void DeleteVideoFile()
    {
        if (File.Exists(_videoPath))
        {
            File.Delete(_videoPath);
        }
    }
    
    private void SaveVideo()
    {
        _view.videoPlayer.Play(false);
        _signal.Fire(new PopupInputViewSignal(_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.SaveKey) + "?", _cultureCode, Save));
        
        void Save(bool isSubmit, string name)
        {
            if (isSubmit)
            {
                SetButtonInteractable(_view.saveButton, false);
#if UNITY_WEBGL && !UNITY_EDITOR
                var videoExtension = FileExtension.DefaultWebVideoExtension;
#else
                var videoExtension = FileExtension.DefaultWebVideoExtension;
#endif
                string savedTo = _fileManager.CopyFileToMyContentFolder(_videoPath, name, videoExtension);
                Debug.Log($"Video local save: path: {_videoPath}, saved to: {savedTo}");
                _signal.Fire(new PopupOverlaySignal(true, $"{_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.LocalFileSavedKey)} {savedTo}", type: PopupOverlayType.TextBox));
            }
        }
    }
    
    private void SendVideo()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        _asyncProcessor.StartCoroutine(GetByteArrayFromBlobUrl(_videoPath));
#else
        _view.videoPlayer.Play(false);
        Debug.Log($"Video: before read bytes {_videoPath}");
        byte[] fileBytes = File.ReadAllBytes(_videoPath);
        Debug.Log("Video: after read bytes");
        
        _signal.Fire(new PopupInputViewSignal(_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloudKey) + "?", _cultureCode, Send));
        
        void Send(bool isSubmit, string name)
        {
            if(isSubmit)
            {
                SetButtonInteractable(_view.sendButton, false);
                _signal.Subscribe<SaveUserContentServerResponseSignal>(SaveUserContentServerResponse);
                _signal.Fire(new PopupOverlaySignal(true, $"{_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloudKey)}..."));
                _signal.Fire(new SaveUserContentServerCommandSignal(UserContentTypeIDConstants.Video, _assetID, name, name + FileExtension.DefaultVideoExtension, fileBytes));
            }
        }
#endif
    }
    
    private IEnumerator GetByteArrayFromBlobUrl(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error downloading blob: {webRequest.error}");
            }
            else
            {
                byte[] byteArray = webRequest.downloadHandler.data;
                Debug.Log("UnityWebRequest downloaded bytes of video");

                _view.videoPlayer.Play(false);

                _signal.Fire(new PopupInputViewSignal(_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloudKey) + "?", _cultureCode, Send));

                void Send(bool isSubmit, string name)
                {
                    if(isSubmit)
                    {
                        SetButtonInteractable(_view.sendButton, false);
                        _signal.Subscribe<SaveUserContentServerResponseSignal>(SaveUserContentServerResponse);
                        _signal.Fire(new PopupOverlaySignal(true, $"{_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloudKey)}..."));
                        _signal.Fire(new SaveUserContentServerCommandSignal(UserContentTypeIDConstants.Video, _assetID, name, name + FileExtension.DefaultWebVideoExtension, byteArray));
                    }
                }
            }
        }
    }

    private void SetButtonInteractable(Button btn, bool isInteractable)
    {
        btn.interactable = isInteractable;
        btn.transform.Get<Image>(IconName).color = isInteractable ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    private void UpdateHelpPanelLocalization(string msgKey)
    {
        // _view.closeHelpButton.GetComponentInChildren<TextMeshProUGUI>().text = _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloseKey);
        // _view.helpButton.GetComponentInChildren<TextMeshProUGUI>().text = _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.HelpKey);
        // _view.dontShowToggle.GetComponentInChildren<TextMeshProUGUI>().text =_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.DontDisplayThisMessageAgainKey);
        //
        // _view.messageText.text = _localization.GetSystemTranslations(_cultureCode, msgKey);
        //
        // _view.messageText.Rebuild(CanvasUpdate.Layout);
    }

    private void UpdateVideoViewLocalization()
    {
        _view.GetComponentsInChildren<TooltipEvents>(true).ToList().ForEach(te =>
        {
            te.SetHint(_localization.GetSystemTranslations(_cultureCode, te.GetKey()));
        });
        
        _view.GetComponentsInChildren<ToggleTooltipEvents>(true).ToList().ForEach(tte =>
        {
            tte.SetTrueHint(_localization.GetSystemTranslations(_cultureCode, tte.GetTrueKey()));
            tte.SetFalseHint(_localization.GetSystemTranslations(_cultureCode, tte.GetFalseKey()));
        });
    }
    
    public void OnViewEnable(){}

    public void OnViewDisable(){}
}
