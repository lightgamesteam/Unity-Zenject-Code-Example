using System;
using System.Collections;
using TMPro;
using UnityEngine;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Tools;

namespace XDPaint
{
    [DisallowMultipleComponent]
    public class PaintManager3DL : PaintManager
    {
        public PaintTextInputItem textInputTemplate;
        private Material _material;

        public static Action OnDestroyManager = delegate {  };
        private void OnEnable()
        {
            
        }

        protected override void Start()
        {
            
        }
        
        void OnDestroy()
        {
            if (_initialized)
            {
                OnDestroyManager?.Invoke();
                _renderTextureHelper.ReleaseTextures();
                materialsContainer.RestoreTexture();
                DestroyImmediate(_renderTextureHelper.PaintTexture);
                DestroyImmediate(_renderTextureHelper.CombinedTexture);
                DestroyImmediate(materialsContainer.SourceTexture);
                UnsubscribeInputEvents();
            }
        }
        
        public void SetEraseState(bool isErase)
        {
            if(isErase)
                materialsContainer.PaintMode = PaintMode.Erase;
            else
                materialsContainer.PaintMode = PaintMode.Paint;
        }

        public void SetBrushSize(float value)
        {
            materialsContainer.Brush.Size = value;
        }
        
        public void SetBrushColor(Color value)
        {
            materialsContainer.Brush.Color = value;
        }
        
        public void DeselectAllCanvasForPainting()
        {
            RaycastController.Instance._meshesData.Clear();
        }
        
        public void SelectCanvasForPainting()
        {
            RaycastController.Instance._meshesData.Clear();
            
            RaycastController.Instance.InitObject(InputController.Instance.Camera, _renderComponentsHelper.PaintComponent, _renderComponentsHelper.RendererComponent, 
                triangles);
        }
        
        public void Initialize()
        {
            RaycastController.Instance._meshesData.Clear();
            Init();
            if (_initialized)
            {
                _renderComponentsHelper.SetSourceMaterial(_renderComponentsHelper.Material);
            }
        }

        protected override  void InitRenderTexture()
        {
            _renderTextureHelper = new RenderTextureHelper();
            _renderTextureHelper.Init((int)Settings.Instance.defaultTextureWidth, (int)Settings.Instance.defaultTextureHeight, renderTextureMode);
            
            if (renderTextureMode == RenderTexturesMode.OneTexture)
            {
                materialsContainer.SetObjectMaterialTexture(_renderTextureHelper.PaintTexture);
            }
            else if (renderTextureMode == RenderTexturesMode.TwoTextures)
            {
                if (materialsContainer.SourceTexture != null)
                {
                    Graphics.Blit(materialsContainer.SourceTexture, _renderTextureHelper.CombinedTexture);
                }
                materialsContainer.SetObjectMaterialTexture(_renderTextureHelper.CombinedTexture);
            }
            materialsContainer.Paint.SetPaintTexture(_renderTextureHelper.PaintTexture);

            _material = gameObject.GetComponent<Renderer>().material;
            StartCoroutine(ClearRenderTexture());
        }

        public IRenderTextureHelper GetRenderTextureHelper()
        {
            return _renderTextureHelper;
        }

        public MaterialsContainer GetMaterialsContainer()
        {
            return materialsContainer;
        }

        public void ClearPaintTexture()
        {
            StartCoroutine(ClearRenderTexture());
        }

        IEnumerator ClearRenderTexture()
        {
            yield return new WaitForEndOfFrame();
            _renderTextureHelper.PaintTexture.Clear();
            yield return new WaitForEndOfFrame();
            _material.color = Color.white;
        }
    }
}