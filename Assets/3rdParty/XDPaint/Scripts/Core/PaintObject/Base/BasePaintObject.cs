using System.Collections.Generic;
using UnityEngine;
using XDPaint.Core.Materials;
using XDPaint.Core.PaintObject.States;
using XDPaint.Tools;
using XDPaint.Tools.Raycast;

namespace XDPaint.Core.PaintObject.Base
{
    public abstract class BasePaintObject : BasePaintObjectRenderer
    {
        #region Events
        public delegate void PaintHandler(Vector2 paintPosition, float brushSize, float pressure, Color brushColor, PaintMode mode);
        public event PaintHandler OnPaintHandler;
        #endregion

        #region Properties and variables
        public bool IsPainting { get; private set; }

        private Camera _camera;
        public new Camera Camera
        {
            protected get { return _camera; }
            set
            {
                _camera = value;
                base.Camera = _camera;
            }
        }
        protected Vector2? PaintPosition { private get; set; }
        protected Transform ObjectTransform { get; private set; }
        
        private float _pressure = 1f;
        private float Pressure
        {
            get { return Mathf.Clamp(_pressure, 0.01f, 10f); }
            set { _pressure = value; }
        }

        public StateKeeper StateKeeper;
        public bool Preview;

        private LineData _lineData;
        private Vector2 _previousPaintPosition;
        private bool _shouldReDraw;
        private bool _shouldClearTexture = true;
        private bool _haveSourceTexture;
        private bool _haveTwoRenderTextures;
        private const float HalfTextureRatio = 0.5f;
        #endregion

        protected abstract void Init();
        protected abstract void OnPaint(Vector3 position, Vector2? uv = null);

        public void InitPaint(Camera camera, Transform objectTransform, IMaterialsContainer materialsContainer, IRenderTextureHelper renderTextureHelper)
        {
            _camera = camera;
            InitRenderer(Camera, renderTextureHelper, materialsContainer);
            ObjectTransform = objectTransform;
            MaterialsContainer = materialsContainer;
            _haveSourceTexture = MaterialsContainer.SourceTexture != null;
            _haveTwoRenderTextures = renderTextureHelper.CombinedTexture != null;
            _lineData = new LineData();
            InitStateKeeper();
            Init();
        }

        private void InitStateKeeper()
        {
            StateKeeper = new StateKeeper();
            StateKeeper.Init(OnExtraDraw, Settings.Instance.undoRedoEnabled);
            StateKeeper.OnResetState = () => _shouldClearTexture = true;
            StateKeeper.OnChangeState = () => _shouldReDraw = true;
        }
        
        #region Input
        public void OnMouseHover(Vector3 position, Triangle triangle = null)
        {
            if (!ObjectTransform.gameObject.activeInHierarchy)
                return;
            if (!IsPainting)
            {
                if (triangle != null)
                {
                    OnPaint(position, triangle.UVHit);
                }
                else
                {
                    OnPaint(position);
                }
            }
        }

        public void OnMouseDown(Vector3 position, float pressure = 1f, Triangle triangle = null)
        {
            if (!ObjectTransform.gameObject.activeInHierarchy)
                return;

            if (triangle != null && triangle.Transform != ObjectTransform)
            {
                return;
            }
            Pressure = pressure;
            if (PaintPosition != null)
            {
                IsPainting = true;
                StateKeeper.OnMouseDown(new State(MaterialsContainer.PaintMode, MaterialsContainer.Brush.Material.mainTexture, 
                    MaterialsContainer.Brush.Material.color, MaterialsContainer.Brush.Size));
            }
        }

        public void OnMouseButton(Vector3 position, float pressure = 1f, Triangle triangle = null)
        {           
            if (!ObjectTransform.gameObject.activeInHierarchy)
                return;

            if (triangle == null)
            {
                _lineData.AddBrush(pressure * MaterialsContainer.Brush.Size);
                OnPaint(position);
                Pressure = pressure;
                if (!IsPainting && PaintPosition != null)
                {
                    IsPainting = true;
                    StateKeeper.OnMouseDown(new State(MaterialsContainer.PaintMode, MaterialsContainer.Brush.Material.mainTexture, 
                        MaterialsContainer.Brush.Material.color, MaterialsContainer.Brush.Size));
                }
            }
            else if (triangle.Transform == ObjectTransform)
            {
                if (!IsPainting)
                {
                    StateKeeper.OnMouseDown(new State(MaterialsContainer.PaintMode, MaterialsContainer.Brush.Material.mainTexture, 
                        MaterialsContainer.Brush.Material.color, MaterialsContainer.Brush.Size));
                }
                IsPainting = true;
                _lineData.AddTriangleBrush(triangle, pressure * MaterialsContainer.Brush.Size);
                Pressure = pressure;
                OnPaint(position, triangle.UVHit);
            }
            else
            {
                PaintPosition = null;
                _lineData.Clear();
            }
        }
        
        public void OnMouseUp(Vector3 position)
        {
            if (!ObjectTransform.gameObject.activeInHierarchy)
                return;
            FinishPainting();
        }
        #endregion

        public void FinishPainting()
        {
            if (IsPainting)
            {
                Pressure = 1f;
                IsPainting = false;
                StateKeeper.OnMouseUp();
                _lineData.Clear();
            }
            MaterialsContainer.Paint.SetPaintPreviewVector(Vector4.zero);
            PaintPosition = null;
        }

        public void OnRender()
        {
            if (IsPainting && PaintPosition != null)
            {
                if (_lineData.HasOnePosition() && _previousPaintPosition != PaintPosition.Value)
                {
                    DrawPoint();
                    _previousPaintPosition = PaintPosition.Value;
                }
                else if (!_lineData.HasOnePosition())
                {
                    if (_lineData.HasNotSameTriangles())
                    {
                        DrawLine(false);
                    }
                    else
                    {
                        DrawLine(true);
                    }
                }
            }
            
            if (_shouldClearTexture)
            {
                ClearRenderTexture();
                if (!_haveTwoRenderTextures)
                {
                    RenderCombined();
                }
                _shouldClearTexture = false;
            }

            if (_shouldReDraw)
            {
                StateKeeper.OnReDraw();
                _shouldReDraw = false;
            }
        }

        private Rect GetPosition(Vector2 holePosition, float scale)
        {
            var positionX = (int) holePosition.x;
            var positionY = (int) holePosition.y;
            var positionRect = new Rect(
                (positionX - HalfTextureRatio * MaterialsContainer.Brush.Material.mainTexture.width * scale) /
                MaterialsContainer.SourceTextureWidth,
                (positionY - HalfTextureRatio * MaterialsContainer.Brush.Material.mainTexture.width * scale) /
                MaterialsContainer.SourceTextureHeight,
                MaterialsContainer.Brush.Material.mainTexture.width / (float)MaterialsContainer.SourceTextureWidth * scale,
                MaterialsContainer.Brush.Material.mainTexture.height / (float)MaterialsContainer.SourceTextureHeight * scale
            );
            return positionRect;
        }

        private void OnExtraDraw(int startFromIndex, int drawingActionsIndex, Dictionary<int, State> drawingActions)
        {
            var paintMode = MaterialsContainer.PaintMode;
            var brushColor = MaterialsContainer.Brush.Color;
            var brushTexture = MaterialsContainer.Brush.Texture;
            var preview = Preview;
            for (var i = startFromIndex; i < drawingActionsIndex; i++)
            {
                MaterialsContainer.Preview = false;
                MaterialsContainer.PaintMode = drawingActions[i].PaintMode;
                MaterialsContainer.Brush.Color = drawingActions[i].BrushColor;
                MaterialsContainer.Brush.Texture = drawingActions[i].BrushTexture;
                
                foreach (var actions in drawingActions[i].DrawingStates)
                {
                    if (actions.Positions.Length > 1)
                    {
                        RenderLine(actions.Positions, drawingActions[i].BrushTexture, drawingActions[i].BrushSize, actions.BrushSizes);
                    }
                    else
                    {
                        var positionRect = GetPosition(actions.Positions[0], actions.BrushSizes[0]);
                        UpdateQuad(positionRect);
                    }
                }
            }
            MaterialsContainer.PaintMode = paintMode;
            MaterialsContainer.Brush.Color = brushColor;
            MaterialsContainer.Brush.Texture = brushTexture;
            MaterialsContainer.Preview = preview;
        }

        private void DrawPoint()
        {
            var positionRect = GetPosition(PaintPosition.Value, MaterialsContainer.Brush.Size * Pressure);
            UpdateQuad(positionRect);
            if (IsPainting)
            {
                StateKeeper.AddState(PaintPosition.Value, MaterialsContainer.Brush.Size * Pressure);
            }
        }

        public void RenderCombined()
        {
            if (_haveTwoRenderTextures)
            {
                ClearCombined();
            }
            else
            {
                PreDrawCombined();
            }
            SetDefaultQuad();
            
            var loopFrom = _haveSourceTexture ? 0 : 1;
            for (int i = loopFrom; i < MaterialsContainer.Paint.Material.passCount; i++)
            {
                DrawCombined(i);
            }
        }

        public void ClearTexture()
        {
            _shouldClearTexture = true;
        }

        private void DrawLine(bool interpolate)
        {
            Vector2[] positions;
            if (interpolate)
            {
                positions = _lineData.GetPositions();
            }
            else
            {
                var paintPositions = _lineData.GetPositions();
                var triangles = _lineData.GetTriangles();
                positions = DrawLine(paintPositions[0], paintPositions[1], triangles[0], triangles[1]);
            }
            if (positions.Length > 0)
            {
                var brushes = _lineData.GetBrushes();
                RenderLine(positions, MaterialsContainer.Brush.Texture, MaterialsContainer.Brush.Size, brushes);
                StateKeeper.AddState(positions, brushes);
            }
        }
        
        protected void OnPostPaint()
        {
            if (PaintPosition != null && IsPainting)
            {
                if (OnPaintHandler != null)
                {
                    OnPaintHandler(PaintPosition.Value, MaterialsContainer.Brush.Size, Pressure, MaterialsContainer.Paint.Material.color, MaterialsContainer.PaintMode);
                }
                _lineData.AddPosition(PaintPosition.Value);
                
                if (Preview)
                {
                    var brushOffset = GetPreviewVector();
                    MaterialsContainer.Paint.SetPaintPreviewVector(brushOffset);
                }
            }
            else if (PaintPosition == null)
            {
                _lineData.Clear();
            }
            
            if (Preview)
            {
                if (PaintPosition != null)
                {
                    var brushOffset = GetPreviewVector();
                    MaterialsContainer.Paint.SetPaintPreviewVector(brushOffset);
                }
                else
                {
                    MaterialsContainer.Paint.SetPaintPreviewVector(Vector4.zero);
                }
            }
        }

        private Vector4 GetPreviewVector()
        {
            var brushRatio = new Vector2(
                                 MaterialsContainer.SourceTextureWidth / (float) MaterialsContainer.Brush.Material.mainTexture.width,
                                 MaterialsContainer.SourceTextureHeight / (float) MaterialsContainer.Brush.Material.mainTexture.height) / MaterialsContainer.Brush.Size / Pressure;
            var brushOffset = new Vector4(
                PaintPosition.Value.x / MaterialsContainer.SourceTextureWidth * brushRatio.x,
                PaintPosition.Value.y / MaterialsContainer.SourceTextureHeight * brushRatio.y,
                1f / brushRatio.x, 1f / brushRatio.y);
            return brushOffset;
        }
    }
}