using UnityEngine;
using XDPaint.Core.PaintObject.Base;

namespace XDPaint.Core.PaintObject
{
    public sealed class CanvasRendererPaint : BasePaintObject
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private Vector2 _scratchBoundsSize;
        private RenderMode _renderMode;

        protected override void Init()
        {
            _canvas = ObjectTransform.transform.GetComponentInParent<Canvas>();
            _rectTransform = ObjectTransform.GetComponent<RectTransform>();
            GetScratchBounds();
        }

        private bool IsInBounds(Vector3 position)
        {
            Vector2 clickPosition = position;
            var bounds = new Bounds(_rectTransform.position, Vector2.Scale(_rectTransform.rect.size, ObjectTransform.lossyScale));
            bounds.center = new Vector3(bounds.center.x, bounds.center.y, 0);
            var density = new Vector3(
                              MaterialsContainer.SourceTextureWidth / (float)MaterialsContainer.Brush.Material.mainTexture.width * MaterialsContainer.Brush.Size / bounds.size.x,
                              MaterialsContainer.SourceTextureHeight / (float)MaterialsContainer.Brush.Material.mainTexture.height * MaterialsContainer.Brush.Size / bounds.size.y) / 2;
            
            if (_renderMode == RenderMode.ScreenSpaceOverlay)
            {
                bounds.size += new Vector3(MaterialsContainer.Brush.Material.mainTexture.width, MaterialsContainer.Brush.Material.mainTexture.height);
            }
            if (_renderMode == RenderMode.ScreenSpaceCamera)
            {
                clickPosition = Camera.ScreenToWorldPoint(position);
                bounds.size += density;
            }
            if (_renderMode == RenderMode.WorldSpace)
            {
                bounds.center = _rectTransform.position;
                var ray = Camera.ScreenPointToRay(clickPosition);
                return bounds.IntersectRay(ray);
            }
            return bounds.Contains(clickPosition);
        }

        private void GetScratchBounds()
        {
            if (_rectTransform != null)
            {
                var rect = _rectTransform.rect;
                var lossyScale = _rectTransform.lossyScale;
                _scratchBoundsSize = new Vector2(rect.size.x * lossyScale.x, rect.size.y * lossyScale.y);
                if (_canvas != null)
                {
                    _renderMode = _canvas.renderMode;
                }
                else
                {
                    Debug.LogWarning("Can't find Canvas component in parent GameObjects!");
                }
            }
        }

        protected override void OnPaint(Vector3 position, Vector2? uv = null)
        {
            if (!IsInBounds(position))
            {
                PaintPosition = null;
                OnPostPaint();
                return;
            }
            
            Vector3 clickPosition;
            if (_renderMode == RenderMode.ScreenSpaceOverlay)
            {
                clickPosition = position;
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, position, Camera))
                {
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, position, Camera, out clickPosition);
                }
                else
                {
                    PaintPosition = null;
                    OnPostPaint();
                    return;
                }
            }

            var surfaceLocalClickPosition = ObjectTransform.InverseTransformPoint(clickPosition);
            var lossyScale = ObjectTransform.lossyScale;
            var clickLocalPosition = new Vector2(surfaceLocalClickPosition.x * lossyScale.x, surfaceLocalClickPosition.y * lossyScale.y);
            GetScratchBounds();
            var scratchSurfaceClickLocalPosition = clickLocalPosition + _scratchBoundsSize / 2f;
            var ppi = new Vector2(
                MaterialsContainer.SourceTextureWidth / _scratchBoundsSize.x / lossyScale.x,
                MaterialsContainer.SourceTextureHeight / _scratchBoundsSize.y / lossyScale.y);
            PaintPosition = new Vector2(
                scratchSurfaceClickLocalPosition.x * lossyScale.x * ppi.x,
                scratchSurfaceClickLocalPosition.y * lossyScale.y * ppi.y);
            OnPostPaint();
        }
    }
}