using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class AsyncProcessorService : MonoBehaviour, IAsyncProcessorService
{
    [Inject] public readonly SignalBus Signal;

    public static AsyncProcessorService Instance;

    public bool ApplicationInFocus { get; private set; } = true;
    public bool ApplicationOnPause { get; private set; } = false;
    
    public Action<bool> OnApplicationFocusAction = delegate {  };
    public Action<bool> OnApplicationPauseAction = delegate {  };
    public Action OnApplicationQuitAction = delegate {  };
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        if (!Application.isEditor || Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        OnApplicationFocusAction.Invoke(hasFocus);
        ApplicationInFocus = hasFocus;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        OnApplicationPauseAction.Invoke(pauseStatus);
        ApplicationOnPause = pauseStatus;
    }

    private void OnApplicationQuit()
    {
        OnApplicationQuitAction.Invoke();
    }

    public void Wait(float time, Action onComplete)
    {
        StartCoroutine(DoWait(time, onComplete));
    }

    IEnumerator DoWait(float time, Action onComplete)
    {
        if (time <= 0)
            yield return new WaitForEndOfFrame();
        else
            yield return new WaitForSecondsRealtime(time);

        onComplete.Invoke();
    }
}