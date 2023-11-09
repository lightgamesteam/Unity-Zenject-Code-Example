using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CheckCameraPermissionCommand : ICommandWithParameters
{
    [Inject] private AsyncProcessorService _asyncProcessor;
    
    private Action<bool> _callback;
    private WebCamTexture _camera;

    public void Execute(ISignal signal)
    {
        var parameter = (CheckCameraPermissionCommandSignal) signal;
        _callback = parameter.Callback;
        
        if (HasCameraPermission())
        {
            _callback?.Invoke(true);
        }
        else
        {
            _asyncProcessor.StartCoroutine(Check());
        }
    }
    
    private IEnumerator Check()
    {
#if UI_IOS
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
#endif      
        
#if UNITY_ANDROID
        UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
        
        yield return new WaitForSeconds(0.5f);
        
        float time = Time.time;
        yield return new WaitUntil(() => _asyncProcessor.ApplicationInFocus || HasCameraPermission());
#endif
        
        yield return new WaitForSeconds(0.5f);
        _callback?.Invoke(HasCameraPermission());
    }

    private bool HasCameraPermission()
    {
#if UNITY_EDITOR
        return true;
#endif
        
#if UNITY_ANDROID
        return UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera);
#endif

#if UI_IOS
        return Application.HasUserAuthorization(UserAuthorization.WebCam);
#endif
        
        return false;
    }
}

public class CheckCameraPermissionCommandSignal : ISignal
{
    public Action<bool> Callback { get; }

    public CheckCameraPermissionCommandSignal(Action<bool> callback)
    {
        Callback = callback;
    }
}