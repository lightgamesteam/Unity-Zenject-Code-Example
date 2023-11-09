using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using TDL.Constants;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

public enum RecordingState 
{
    None,
    StartRecording,
    StopRecording
}

[RequireComponent(typeof(AudioSource))]
public class ScreenRecorderManager : MonoBehaviour
{
    [DllImport("__Internal", EntryPoint = @"CheckForPermissions")]
    public static extern void CheckForPermissions(Action<IntPtr, int> callback, IntPtr managerPtr);

    [Inject] public readonly SignalBus _signal;

    private AudioSource _microphoneSource;
    
    private IClock _recordingClock;
    private MP4Recorder _videoRecorder;
    private CameraInput3DL _cameraInput;
    private AudioInput _audioInput;

    private string _defaultMicrophone = string.Empty;
    private bool _isStarted;
    public RecordingState _recordingState = RecordingState.None;
    //private event Action<RecordingState> _recordingStateEvent;
    private event Action<string> _recordedVideoEvent;
    
    private const int VideoSize1280 = 1280;
    private const int VideoSize720 = 720;
    private const float ar16x9 = 1.77777777778f;
    private const float ar9x16 = 0.5625f;

    private float AspectRatio()
    {
        return (float) Screen.width / Screen.height;
    }

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        _microphoneSource = gameObject.GetComponent<AudioSource>();

        MicrophonePermission();
        
#if UNITY_WSA && !UNITY_EDITOR
        UWP.VideoRecorder.Debug += Debug.Log;
#endif
        
    }

    private void UpdateDefaultAudioDevice()
    {
        _defaultMicrophone = PlayerPrefsExtension.GetString(PlayerPrefsKeyConstants.DefaultMicrophone);
    }

    private void MicrophonePermission()
    {
#if !UNITY_WEBGL
        UpdateDefaultAudioDevice();
        Microphone.Start(_defaultMicrophone, true, 5, 44100);
        StartCoroutine(StopMicrophonePermission());
#endif
    }

    public IEnumerator RequestMicrophoneAccess()
    {
        Debug.Log("Requested access from coroutine");
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Has authorization");
            DOVirtual.DelayedCall(2f, StartRecording);
        }
        else
        {
            Debug.Log("Microphone Access is not granted");
        }
    }

    public bool IsUWPCanRecord()
    {
#if UNITY_WSA && !UNITY_EDITOR
        return UWP.VideoRecorder.IsCanRecord();
#endif

        return false;
    }
    
    public string GetUWPStatusError()
    {
        string error = string.Empty;
        
#if UNITY_WSA && !UNITY_EDITOR
        error = UWP.VideoRecorder.GetStatusError();
#endif

        return error;
    }

    IEnumerator StopMicrophonePermission()
    {
        yield return new WaitForSeconds(3);
#if !UNITY_WEBGL
        Microphone.End(_defaultMicrophone);
#endif
    }
    
    #region Public methods

    public RecordingState GetCurrentState() 
    {
        return _recordingState;
    }

    private int GetScreenWidth()
    {
        int w;
        
        if (Screen.width > Screen.height)
        {
            w = VideoSize1280; // landscape
        }
        else
        {
            w = (int) (VideoSize720 * (AspectRatio() / ar9x16)); // portrait
        }

        if (w % 2 > 0)
        {
            w += 1;
        }

        return w;
    }
    
    private int GetScreenHeight()
    {
        int h;
        
        if (Screen.height > Screen.width)
        {
            h = VideoSize1280; // portrait
        }
        else
        {
            h = (int) (VideoSize720 * (ar16x9 / AspectRatio())); // landscape
        }

        if (h % 2 > 0)
        {
            h += 1;
        }

        return h;
    }

#if UNITY_WEBGL && !UNITY_EDITOR

    [MonoPInvokeCallback(typeof(Action<IntPtr, int>))]
    public static void TryStartRecordingWebGL(IntPtr managerPtr, int result)
    {
        var handle = GCHandle.FromIntPtr(managerPtr);
        var manager = (ScreenRecorderManager) handle.Target;
        handle.Free();

        var boolResult = Convert.ToBoolean(result);
        Debug.Log(boolResult);
        
        if (!boolResult)
        {
            manager._signal.Fire(new PopupOverlaySignal(true, 
                "Error: Please, check your microphone permissions!",
                type: PopupOverlayType.TextBox));
            manager._recordingState = RecordingState.StopRecording;
            manager._signal.Fire(new VideoRecordingStateSignal(manager._recordingState));
            Debug.Log("---> Wrong permission");
            return;
        }
        
        manager.StartRecordingWebGl();
    }

    public void StartRecordingWebGl()
    {
        if (_isStarted) { StopRecording(); }

        _isStarted = true;
        // Start recording
        _recordingClock = new RealtimeClock();

        var sampleRate = AudioSettings.outputSampleRate;
        var channelCount = (int)AudioSettings.speakerMode;
        var width = GetScreenWidth();
        var height = GetScreenHeight();
        float rate = 1.0f;
        int bitrate = (int)(width * height * 4 * rate);

        // Create recording inputs
        UpdateDefaultAudioDevice();
        StartMicrophone();
        
        _videoRecorder = new MP4Recorder(
            width,
            height,
            25,
            sampleRate,
            channelCount,
            OnReplay,
            bitrate
        );

        _cameraInput = new CameraInput3DL(_videoRecorder, _recordingClock);
        _audioInput = new AudioInput(_videoRecorder, _recordingClock, _microphoneSource, true);

        _recordingState = RecordingState.StartRecording;
        _signal.Fire(new VideoRecordingStateSignal(_recordingState)); 
    }
#endif
    
    public void StartRecording() 
    {
#if UNITY_WSA && !UNITY_EDITOR
        UWP.VideoRecorder.StartRecord();
        _isStarted = true;
        _recordingState = RecordingState.StartRecording;
        _signal.Fire(new VideoRecordingStateSignal(_recordingState));
        return;
#endif
        
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("Called from c# -> before handle");
        var handle = GCHandle.Alloc(this);
        Debug.Log("Called from c# -> after handle");
        CheckForPermissions(TryStartRecordingWebGL, GCHandle.ToIntPtr(handle));
        Debug.Log("Called from c# -> after Check");
        return;
#endif
        if (_isStarted) { StopRecording(); }

        _isStarted = true;
        // Start recording
        _recordingClock = new RealtimeClock();

        var sampleRate = AudioSettings.outputSampleRate;
        var channelCount = (int)AudioSettings.speakerMode;
        var width = GetScreenWidth();
        var height = GetScreenHeight();
        float rate = 1.0f;
        int bitrate = (int)(width * height * 4 * rate);

        // Create recording inputs
        UpdateDefaultAudioDevice();
        StartMicrophone();
        
        _videoRecorder = new MP4Recorder(
            width,
            height,
            25,
            sampleRate,
            channelCount,
            OnReplay,
            bitrate
        );

        _cameraInput = new CameraInput3DL(_videoRecorder, _recordingClock);
        _audioInput = new AudioInput(_videoRecorder, _recordingClock, _microphoneSource, true);

        _recordingState = RecordingState.StartRecording;
        _signal.Fire(new VideoRecordingStateSignal(_recordingState));
    }

    public void StopRecording() 
    {
        if(!_isStarted) { return; }
        
#if UNITY_WSA && !UNITY_EDITOR
       string path = UWP.VideoRecorder.StopRecord();
        _isStarted = false;
        _recordingState = RecordingState.StopRecording;
        _signal.Fire(new VideoRecordingStateSignal(_recordingState));
       _recordedVideoEvent.Invoke(path);
        return;
#endif
        // Stop the recording inputs
        StopMicrophone();
        _audioInput?.Dispose();
        _cameraInput?.Dispose();
        // Stop recording
        _videoRecorder?.Dispose();
        _isStarted = false;
        _recordingState = RecordingState.StopRecording;
        _signal.Fire(new VideoRecordingStateSignal(_recordingState));
        //_recordingStateEvent?.Invoke(_recordingState);
    }

    public bool IsRecording()
    {
        return _isStarted;
    }

    public void AddRecordedVideoListener(Action<string> action) 
    {
        _recordedVideoEvent += action;
    }
    
    public void RemoveRecordedVideoListener(Action<string> action) 
    {
        if (_recordedVideoEvent != null) {
            _recordedVideoEvent -= action;            
        }
    }
    
    #endregion

    #region Private methods

    private void StartMicrophone() 
    {
        #if !UNITY_WEBGL || UNITY_EDITOR // No `Microphone` API on WebGL :(
        // Create a microphone clip
        _microphoneSource.clip = Microphone.Start(_defaultMicrophone, false, 1800, 44100); //48000
        while (Microphone.GetPosition(null) <= 0) { }

        // Play through audio source
        _microphoneSource.timeSamples = Microphone.GetPosition(null);
        _microphoneSource.loop = true;
        _microphoneSource.PlayDelayed(0.1f);
        #endif
    }

    private void StopMicrophone() 
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
        Microphone.End(_defaultMicrophone);
        _microphoneSource.Stop();
        #endif
    }

    private void OnReplay(string path) 
    {
        Debug.Log("Saved recording to: " + path);
        _recordedVideoEvent?.Invoke(path);
    }
    
    #endregion
}