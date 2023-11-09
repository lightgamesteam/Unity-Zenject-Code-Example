using System;
using UnityEngine;
using UnityEngine.UI;

public class WebCameraRenderer : MonoBehaviour, IWebCameraRenderer
{
    private bool _camAvailable;
    private WebCamTexture _backCam;

    private RawImage _background;
    private AspectRatioFitter _aspectRatio;

    void OnEnable()
    {
        CacheLinks();
        InitWebCamera();
    }

    private void OnDisable()
    {
        StopWebCamera();
    }

    private void OnDestroy()
    {
        if(_background.texture != null)
            DestroyImmediate(_background.texture);
    }

    private void CacheLinks()
    {
        _background = GetComponent<RawImage>();
        _aspectRatio = GetComponent<AspectRatioFitter>();
    }

    private void InitWebCamera() {
        var devices = WebCamTexture.devices;

        if (devices.Length == 0) {
            _camAvailable = false;
            return;
        }

        var cameraName = devices[0].name;
        foreach (var device in devices) {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_10_0
            if (device.isFrontFacing) {
                cameraName = device.name;
            }
#else
            if (!device.isFrontFacing) {
                cameraName = device.name;
            }
#endif
            _backCam = new WebCamTexture(cameraName, Screen.width, Screen.height);
        }

        if (_backCam == null) {
            return;
        }

        _camAvailable = true;
        _background.texture = _backCam;
        StartWebCamera();
    }

    private void Update()
    {
        if (_camAvailable && _backCam.isPlaying)
        {
            var ratio = (float) _backCam.width / _backCam.height;
            _aspectRatio.aspectRatio = ratio;

            var scaleY = _backCam.videoVerticallyMirrored ? -1.0f : 1.0f;
            _background.rectTransform.localScale = new Vector3(1.0f, scaleY, 1.0f);

            var orientation = -_backCam.videoRotationAngle;
            _background.rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, orientation);
        }
    }

    private void StartWebCamera()
    {
        _backCam.Play();
        //_background.enabled = true;
    }

    private void PauseWebCamera()
    {
        _backCam.Pause();
        //_background.enabled = false;
    }

    public void ToggleWebCamera()
    {
        if (_camAvailable)
        {
            if (_backCam.isPlaying)
            {
                PauseWebCamera();
            }
            else
            {
                StartWebCamera();
            }
        }
    }

    public void StopWebCamera()
    {
        if (_camAvailable)
        {
            _backCam.Stop();
            //_background.enabled = false;
        }
    }
}