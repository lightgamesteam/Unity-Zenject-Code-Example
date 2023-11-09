using System;
using System.Linq;
using UnityEngine;

public class SceneFrameManager : MonoBehaviour {
    private RenderTexture _frameRenderTexture;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    
    #region SceneFrameCache
    
    public bool CreateFrameToCache(bool ignoreUi = false) {
        _frameRenderTexture = GetFrame(out var result, ignoreUi);
        GC.Collect(0);
        return result;
    }
    
    public RenderTexture GetFrameOfRenderTextureFromCache() {
        return _frameRenderTexture;
    }

    public Texture2D GetFrameOfTexture2dFromCache() {
        return _frameRenderTexture != null ? ConvertToTexture2D(_frameRenderTexture) : default;
    }
    
    #endregion
    
    #region SceneFrame
    
    public RenderTexture GetFrameOfRenderTexture(bool ignoreUi = false) {
        return GetFrame(out var result, ignoreUi);
    }

    public Texture2D GetFrameOfTexture2d(bool ignoreUi = false) {
        return ConvertToTexture2D(GetFrameOfRenderTexture(ignoreUi));
    }
    
    #endregion

    #region Private methods
    
    private RenderTexture GetFrame(out bool isRendered, bool ignoreUi) {
        var encoderFrame = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        encoderFrame.Create();
        
        var cameras = FindObjectsOfType<Camera>().Where(camera => {
            var cameraGo = camera.gameObject;
            return cameraGo.activeSelf && cameraGo.activeInHierarchy && (!ignoreUi || cameraGo.layer != 5);
        }).ToArray();

        isRendered = false;
        foreach (var camera in cameras) {
            var prevTarget = camera.targetTexture;
            camera.targetTexture = encoderFrame;
            camera.Render();
            camera.targetTexture = prevTarget;
            isRendered = true;
        }
        return encoderFrame;
    }
    
    private static Texture2D ConvertToTexture2D(RenderTexture renderTexture) {
        var result = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        result.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        result.Apply();
        return result;
    }
    
    #endregion
}