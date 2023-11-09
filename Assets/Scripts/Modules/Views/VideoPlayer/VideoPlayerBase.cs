using System;
using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Vimeo.Player;

#if UNITY_IOS
using System.Collections;
using UnityEngine.Apple.ReplayKit;
#endif

public class VideoPlayerBase : ViewBase
{
    [SerializeField] private VimeoPlayer _vimeoPlayer;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _notesButton;
    [SerializeField] private Toggle _playPauseToggle;
    [SerializeField] private Button _screenshotButton;
    [SerializeField] private Toggle _videoRecordingToggle;
    [SerializeField] private Button _helpVideoRecordingButton;
    [SerializeField] private Slider _playbackSpeedSlider;
    [SerializeField] private SelectableKeyboardKeyEvent _keyPlaybackSpeedSlider;
    [SerializeField] private TextMeshProUGUI _playbackSpeedText;
    [SerializeField] private Slider _timeLine;
    [SerializeField] private SelectableKeyboardKeyEvent _keyTimeLine;
    [SerializeField] private GameObject _loadingImage;
    [SerializeField] private PaintView _paintView;

    private RawImage _videoRawImage;
    private bool _isVimeo = true;
    private int assetId = -1;
    public void InitializeModule(string videoPath)
    {
        if (new Uri(videoPath).Host == "vimeo.com")
        {
            _vimeoPlayer.LoadVideo(videoPath);
            _vimeoPlayer.LoadAndPlayVideo();
        }
        else
        {
            _videoPlayer.playOnAwake = false;
            _isVimeo = false;
            _vimeoPlayer.enabled = false;
            _videoPlayer.url = videoPath;
        }

        Debug.Log($"Video Path = {videoPath}");
        // Fix - UN-1781 No Audio -
        //_videoPlayer.EnableAudioTrack(0, false);
        //_videoPlayer.audioOutputMode = VideoAudioOutputMode.APIOnly;
        // ---------------------------
        
        _videoRawImage = _videoPlayer.GetComponent<RawImage>();
        _playPauseToggle.SetValue(false, false);

        _videoPlayer.skipOnDrop = DeviceInfo.IsAndroid() || DeviceInfo.IsChromebook() || DeviceInfo.IsIOS() || DeviceInfo.IsTablet();
        _videoPlayer.prepareCompleted += (source) => Play();
        _videoPlayer.errorReceived += (videoPlayer, message) =>
        {
            Debug.Log($"Play Movie Error: {message}");
        };

        _videoPlayer.loopPointReached += source =>
        {
            AsyncProcessorService.Instance.Wait(0, () => _timeLine.normalizedValue = 1f);
        };
        
        _videoPlayer.Prepare();
        InitializePaint();
    }
    
    private void InitializePaint()
    {
        if (_paintView != null)
        {
            _paintView.paintViewType = PaintViewType.VideoView;
            _paintView.camera = _paintView.gameObject.GetFirstInScene<Camera>();

            _paintView?.paint3DToggle?.onValueChanged?.AddListener(v =>
            {
                _videoRawImage.raycastTarget = !v;
                _playPauseToggle.isOn = v;
            });
            
            AsyncProcessorService.Instance.Wait(0.1f, () =>
            {
                Signal.Fire(new InitializePaintSignal(_paintView));
            });
        }
    }
    
    public void ClearTexture()
    {
        _videoPlayer.targetTexture.Clear();
    }

    public void OnDisable()
    {
        if(_videoPlayer)
            _videoPlayer.playOnAwake = false;
    }

    public override void SubscribeOnListeners()
    {
        _closeButton?.onClick.AddListener(CloseModule);
        _notesButton?.onClick.AddListener(ShowStudentNotes);
        _screenshotButton?.onClick.AddListener(TakeScreenshot);
        _videoRecordingToggle?.onValueChanged.AddListener(StartVideoRecording);
        _helpVideoRecordingButton?.onClick.AddListener(HelpVideoRecording);
        _playbackSpeedSlider.onValueChanged.AddListener(SetPlaybackSpeed);
        _keyPlaybackSpeedSlider.onKeyDown.AddListener(KeyPlaybackSpeed);
        _keyTimeLine.onKeyDown.AddListener(KeyTimeLine);

        _playPauseToggle.onValueChanged.AddListener(PlayOn);

        _videoPlayer.loopPointReached += VideoOver;
        _videoPlayer.prepareCompleted += VideoReady;
    }

    private void KeyPlaybackSpeed(KeyCode kc)
    {
        if (kc == KeyCode.LeftArrow)
        {
            _playbackSpeedSlider.value -= 0.2f;
        }
        
        if (kc == KeyCode.RightArrow)
        {
            _playbackSpeedSlider.value += 0.2f;
        }
    }
    
    private void KeyTimeLine(KeyCode kc)
    {
        float seekDir = 0;

        if (kc == KeyCode.LeftArrow)
        {
            seekDir = -_videoPlayer.frameRate * 3;
        }
        else if (kc == KeyCode.RightArrow)
        {
            seekDir = _videoPlayer.frameRate * 3;
        }
        
        if (_vimeoPlayer.enabled)
        {
            _vimeoPlayer.Seek((_videoPlayer.frame + seekDir) / _videoPlayer.frameCount);
        }
        else
        {
            _videoPlayer.frame = (long)(_videoPlayer.frame + seekDir);
        }
    }

    public override void UnsubscribeFromListeners()
    {
        Stop();
        _videoPlayer.targetTexture.Release(); 
    }
    
    public void SetAssetID(int id)
    {
        assetId = id;
    }

    public void SetVideoRecordingToggleVisibility(bool isActive)
    {
        _videoRecordingToggle.gameObject.SetActive(isActive);
    }
    
    public void SetStudentNoteVisibility(bool isActive)
    {
        _notesButton.gameObject.SetActive(isActive);
    }
    
    private void ShowStudentNotes()
    {
        Signal.Fire(new ShowStudentNotesPanelViewSignal(assetId));
    }

    private void HelpVideoRecording()
    {
        Signal.Fire(new OpenUrlCommandSignal(ServerConstants.HelpVideoRecordingUrl));
    }
    
    private void StartVideoRecording(bool isStart)
    {
        Signal.Fire(new StartVideoRecordingSignal(isStart, assetId, _videoRecordingToggle));
    }
    
    public void TakeScreenshot() 
    {
        _playPauseToggle.isOn = true;

        Signal.Fire(new TakeScreenshotSignal(assetId));
    }

    internal void OnChangeRecordingState(VideoRecordingStateSignal signal)
    {
        if (signal.State == RecordingState.StartRecording)
        {

            _closeButton.interactable = false;
            _closeButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.35f);
        }
        else
        {
            _playPauseToggle.isOn = true;

            _closeButton.interactable = true;
            _closeButton.GetComponent<Image>().color  = new Color(1, 1, 1, 1);
        }
    }

    public void ToggleVideoPlayback()
    {
        if (_isVimeo)
        {
            _vimeoPlayer.ToggleVideoPlayback();
        }
        else
        {
            PlayOn(_videoPlayer.isPlaying);
        }
    }

    void VideoOver(VideoPlayer vp)
    {
        _playPauseToggle.isOn = true;
    }

    void VideoReady(VideoPlayer vp)
    {
        _loadingImage.SetActive(false);
        _timeLine.maxValue = ((long)vp.frameCount);
    }

    private void CloseModule()
    {
        if(_paintView?.paint3DToggle != null)
            _paintView.paint3DToggle.isOn = false;
        
        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
    
    private void PlayOn(bool isPlay)
    {
        if(isPlay)
            Pause();
        else
            Play();
    }

    private void SetPlaybackSpeed(float speed)
    {
        _videoPlayer.playbackSpeed = speed;
        _playbackSpeedText.text = Math.Round(speed, 1) + "x";
    }
    
    public void Play(bool isPlay)
    {
        _playPauseToggle.InvokeValue(!isPlay, true);
    }
  
    private void Play()
    {
        if(_videoPlayer == null)
            return;
        
        if (_videoPlayer.isPrepared)
        {
            Debug.Log($"Video play url is: {_videoPlayer.url}");
            _videoPlayer.Play();
        }
        else
        {
            if(_videoPlayer != null)
                AsyncProcessorService.Instance.Wait(0.1f, Play);
        }
    }
    
    private void Pause()
    {
        _videoPlayer.Pause();
    }
    
    private void Stop()
    {
        _videoPlayer.Stop();
    }
    
    private double _lastTimePlayed;
    private void Update()
    {
        //when the video is playing, check each time that the video image get update based in the video's frame rate
        if (_videoPlayer.isPlaying && (Time.frameCount % (int)(_videoPlayer.frameRate + 1)) == 0)
        {
            //if the video time is the same as the previous check, that means it's buffering cuz the video is Playing.
            if (_lastTimePlayed == _videoPlayer.time)//buffering
            {
                _loadingImage.SetActive(_videoPlayer.isPlaying);
            }
            else//not buffering
            {
                _loadingImage.SetActive(false);
            }
            
            _lastTimePlayed = _videoPlayer.time;
        }
        
#if UNITY_IOS

        ProcessiOSRecording();
        
#endif

    }
    
    #region iOS recorder

#if UNITY_IOS

    private bool _isReadyForPreview = true;
    private bool _isRecording;

    private void ProcessiOSRecording()
    {
        if (_isReadyForPreview && ReplayKit.recordingAvailable)
        {
            _isReadyForPreview = false;
            _isRecording = false;
			
            ReplayKit.Preview();
            StartCoroutine(DiscardVideo());
        }

        if (!_isRecording && ReplayKit.isRecording)
        {
            Signal.Fire(new VideoRecordingStateSignal(RecordingState.StartRecording));
            _isRecording = true;
        }
    }

    private IEnumerator DiscardVideo()
    {
        yield return new WaitForSeconds(0.5f);
        
        _isReadyForPreview = true;
        ReplayKit.Discard();
    }

#endif
    
    #endregion
}
