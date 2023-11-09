using System.Collections.Generic;
using System.Linq;

namespace NatCorder.Inputs 
{
    using UnityEngine;
    using System;
    using System.Collections;
    using Clocks;
    using Internal;
    
    public sealed class CameraInput3DL : IDisposable 
    {
        public int frameSkip;

        private readonly IMediaRecorder _mediaRecorder;
        private readonly IClock _clock;
        private List<Camera> _cameras = new List<Camera>();
        private readonly CameraInputAttachment _frameHelper;
        private readonly ReadableTexture _frameBuffer;
        private int _frameCount;
        
        #region --Client API--
        
        public CameraInput3DL (IMediaRecorder mediaRecorder, IClock clock) 
        {
            UpdateCameras();
            _mediaRecorder = mediaRecorder;
            _clock = clock;
            var frameDescriptor = new RenderTextureDescriptor(mediaRecorder.pixelWidth, mediaRecorder.pixelHeight, RenderTextureFormat.ARGB32, 24)
            {
                sRGB = true
            };
            
            _frameBuffer = ReadableTexture.ToReadable(RenderTexture.GetTemporary(frameDescriptor));
            
            // Start recording
            _frameHelper = _cameras[0].gameObject.AddComponent<CameraInputAttachment>();
            _frameHelper.StartCoroutine(OnFrame());

            ScreenRecorderCameraInput.OnEnableCamera += UpdateCamera;
        }

        public void Dispose () 
        {
            ScreenRecorderCameraInput.OnEnableCamera = null;
            CameraInputAttachment.Destroy(_frameHelper);
            RenderTexture.ReleaseTemporary(_frameBuffer);
        }
        
        #endregion

        #region --Operations--

        public void UpdateCameras()
        {
            _cameras = MonoBehaviour.FindObjectsOfType<Camera>().ToList();
            _cameras.Sort((a, b) => a.depth.CompareTo(b.depth));
        }
        
        public void UpdateCamera(Camera camera, bool isEnable)
        {
            if (isEnable)
            {
                if (!_cameras.Contains(camera))
                {
                    _cameras.Add(camera);
                    SortCameras();
                }
            }
            else
            {
                if (_cameras.Contains(camera))
                {
                    _cameras.Remove(camera);
                    SortCameras();
                }
            }

            void SortCameras()
            {
                _cameras.RemoveAll(c => c == null);
                _cameras.Sort((a, b) => a.depth.CompareTo(b.depth));
            }
        }
        
        private IEnumerator OnFrame () 
        {
            var yielder = new WaitForEndOfFrame();

            for(;;)
            {
                yield return yielder;
                
                var recordFrame = _frameCount++ % (frameSkip + 1) == 0;
                
                if (recordFrame) 
                {
                    foreach (Camera cam in _cameras)
                    {
                        if (cam != null)
                        {
                            var prevActive = RenderTexture.active;
                            var prevTarget = cam.targetTexture;
                            cam.targetTexture = _frameBuffer;
                            cam.Render();
                            cam.targetTexture = prevTarget;
                            RenderTexture.active = prevActive;
                        }
                    }
                    
                    var timestamp = _clock.Timestamp;
                    _frameBuffer.Readback(pixelBuffer => _mediaRecorder.CommitFrame(pixelBuffer, timestamp));
                }
            }
        }

        private sealed class CameraInputAttachment : MonoBehaviour { }
        #endregion
    }
}