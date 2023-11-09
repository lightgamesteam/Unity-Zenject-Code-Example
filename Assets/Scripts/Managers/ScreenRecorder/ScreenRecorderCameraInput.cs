using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenRecorderCameraInput : MonoBehaviour
{
    public static Action<Camera, bool> OnEnableCamera = delegate {  };

    private Camera _camera;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        OnEnableCamera.Invoke(_camera, true);
    }

    private void OnDisable()
    {
        OnEnableCamera.Invoke(_camera, false);
    }
}
