using UnityEngine;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Core.PaintObject;
using XDPaint.Core.PaintObject.Base;
using XDPaint.Tools;
using XDPaint.Tools.Raycast;

namespace XDPaint
{
    [DisallowMultipleComponent]
    public class PaintManager : MonoBehaviour
    {
        public GameObject objectForPainting;
        public MaterialsContainer materialsContainer;
        public RenderTexturesMode renderTextureMode;
        public bool shouldOverrideCamera;
        
        public BasePaintObject PaintObject { get; private set; }

        [SerializeField] private Camera overrideCamera;
        public Camera Camera
        {
            private get { return (shouldOverrideCamera || overrideCamera == null) ? Camera.main : overrideCamera; }
            set
            {
                overrideCamera = value;
                if (InputController.Instance != null)
                {
                    InputController.Instance.Camera = overrideCamera;
                }
                if (RaycastController.Instance != null)
                {
                    RaycastController.Instance.Camera = overrideCamera;
                }
                if (_initialized)
                {
                    PaintObject.Camera = overrideCamera;
                }
            }
        }
        
        [SerializeField] private bool preview;
        public bool Preview
        {
            get { return preview; }
            set
            {
                preview = value;
                materialsContainer.Preview = preview;
                if (_initialized)
                {
                    PaintObject.Preview = preview;
                    UpdatePreviewInput();
                }
            }
        }
        
        [SerializeField] protected bool useNeighborsVerticesForRaycasts;
        public bool UseNeighborsVerticesForRaycasts
        {
            get { return useNeighborsVerticesForRaycasts; }
            set
            {
                useNeighborsVerticesForRaycasts = value;
                if (!Application.isPlaying)
                {
                    if (useNeighborsVerticesForRaycasts)
                    {
                        FillTrianglesData();
                    }
                    else
                    {
                        ClearTrianglesNeighborsData();
                    }
                }
                if (_initialized)
                {
                    PaintObject.UseNeighborsVertices = useNeighborsVerticesForRaycasts;
                }
            }
        }

        public bool HaveTriangles
        {
            get { return triangles != null && triangles.Length > 0; }
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        [SerializeField] protected Triangle[] triangles;
        protected IRenderTextureHelper _renderTextureHelper;
        protected IRenderComponentsHelper _renderComponentsHelper;
        public bool _initialized;

        public PaintManager()
        {
            if (materialsContainer == null)
            {
                materialsContainer = new MaterialsContainer();
            }
        }
        
        private void OnEnable()
        {
            if (_initialized)
            {
                _renderComponentsHelper.SetSourceMaterial(_renderComponentsHelper.Material);
            }
        }


        protected virtual void Start()
        {
            Init();
        }

        private void Update()
        {
            if (_initialized && (PaintObject.IsPainting || Preview))
            {
                Render();
            }
        }
        
        private void OnDestroy()
        {
            if (_initialized)
            {
                _renderTextureHelper.ReleaseTextures();
                materialsContainer.RestoreTexture();
                UnsubscribeInputEvents();
            }
        }
        
        public void Init()
        {
            if (_initialized)
                return;
            
            if (objectForPainting == null)
            {
                return;
            }
            
            if (_renderComponentsHelper == null)
            {
                _renderComponentsHelper = new RenderComponentsHelper();
            }
            ObjectComponentType componentType;
            _renderComponentsHelper.Init(objectForPainting, out componentType);
            if (componentType == ObjectComponentType.Unknown)
            {
                return;
            }
            
            if (ControllersContainer.Instance == null)
            {
                GameObject containerGameObject = new GameObject(Settings.Instance.containerGameObjectName);
                containerGameObject.AddComponent<ControllersContainer>();
            }
            
            if (_renderComponentsHelper.IsMesh())
            {
                var paintComponent = _renderComponentsHelper.PaintComponent;
                var renderComponent = _renderComponentsHelper.RendererComponent;
                var mesh = _renderComponentsHelper.GetMesh();
                if (triangles == null || triangles.Length == 0)
                {
                    if (mesh != null)
                    {
                        Debug.LogWarning("PaintManager does not have triangles data! Getting it may take a while.");
                        triangles = mesh.GetTrianglesData(useNeighborsVerticesForRaycasts);
                    }
                    else
                    {
                        Debug.LogError("Mesh is null!");
                        return;
                    }
                }
                RaycastController.Instance.InitObject(Camera, paintComponent, renderComponent, triangles);
            }
            materialsContainer.Init(_renderComponentsHelper, Preview);
            InitRenderTexture();
            InitPaintObject();
            InputController.Instance.Camera = Camera;
            SetInputEvents();
            _initialized = true;
            Render();
        }

        public void Render()
        {
            if (_initialized)
            {
                PaintObject.OnRender();
                if (renderTextureMode == RenderTexturesMode.TwoTextures)
                {
                    PaintObject.RenderCombined();
                }
            }
        }
        
        public void FillTrianglesData(bool fillNeighbors = true)
        {
            if (_renderComponentsHelper == null)
            {
                _renderComponentsHelper = new RenderComponentsHelper();
            }
            ObjectComponentType componentType;
            _renderComponentsHelper.Init(objectForPainting, out componentType);
            if (componentType == ObjectComponentType.Unknown)
            {
                return;
            }
            if (_renderComponentsHelper.IsMesh())
            {
                var mesh = _renderComponentsHelper.GetMesh();
                if (mesh != null)
                {
                    triangles = mesh.GetTrianglesData(fillNeighbors);
                    Debug.Log("Added triangles data. Triangles count: " + triangles.Length);
                }
                else
                {
                    Debug.LogError("Mesh is null!");
                }
            }
        }
        
        public void ClearTrianglesData()
        {
            triangles = null;
        }

        public void ClearTrianglesNeighborsData()
        {
            if (triangles != null)
            {
                foreach (var triangle in triangles)
                {
                    triangle.N.Clear();
                }
            }
        }
        
        public RenderTexture GetRenderTexture()
        {
            if (!_initialized)
                return null;
            if (renderTextureMode == RenderTexturesMode.OneTexture)
            {
                return _renderTextureHelper.PaintTexture;
            }
            return _renderTextureHelper.CombinedTexture;
        }

        public Texture2D GetResultTexture()
        {
            var material = materialsContainer.Paint.Material;
            var sourceTexture = material.mainTexture;
            var previousRenderTexture = RenderTexture.active;
            var texture2D = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = GetRenderTexture();
            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0, false);
            RenderTexture.active = previousRenderTexture;
            return texture2D;
        }
        
        public void Bake()
        {
            PaintObject.FinishPainting();
            Render();
            var prevRenderTexture = RenderTexture.active;
            var renderTexture = GetRenderTexture();
            RenderTexture.active = renderTexture;
            if (materialsContainer.SourceTexture != null)
            {
                var texture = materialsContainer.SourceTexture as Texture2D;
                if (texture != null)
                {
                    texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                    texture.Apply();
                }
            }
            RenderTexture.active = prevRenderTexture;
        }

        protected virtual void InitRenderTexture()
        {
            _renderTextureHelper = new RenderTextureHelper();
            _renderTextureHelper.Init(materialsContainer.SourceTextureWidth, materialsContainer.SourceTextureHeight, renderTextureMode);
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
        }

        private void InitPaintObject()
        {
            if (PaintObject == null)
            {
                if (_renderComponentsHelper.ComponentType == ObjectComponentType.CanvasImage)
                {
                    PaintObject = new CanvasRendererPaint();
                }
                else if (_renderComponentsHelper.ComponentType == ObjectComponentType.SpriteRenderer)
                {
                    PaintObject = new SpriteRendererPaint();
                }
                else
                {
                    PaintObject = new MeshRendererPaint();
                }
            }
            PaintObject.InitPaint(Camera, objectForPainting.transform, materialsContainer, _renderTextureHelper);
            PaintObject.Preview = Preview;
            PaintObject.UseNeighborsVertices = UseNeighborsVerticesForRaycasts;
        }

        private void SetInputEvents()
        {
            UpdatePreviewInput();
            if (_renderComponentsHelper.IsMesh())
            {
                InputController.Instance.OnMouseDownWithHit -= PaintObject.OnMouseDown;
                InputController.Instance.OnMouseDownWithHit += PaintObject.OnMouseDown;
                InputController.Instance.OnMouseButtonWithHit -= PaintObject.OnMouseButton;
                InputController.Instance.OnMouseButtonWithHit += PaintObject.OnMouseButton;
            }
            else
            {
                InputController.Instance.OnMouseDown -= PaintObject.OnMouseDown;
                InputController.Instance.OnMouseDown += PaintObject.OnMouseDown;
                InputController.Instance.OnMouseButton -= PaintObject.OnMouseButton;
                InputController.Instance.OnMouseButton += PaintObject.OnMouseButton;
            }
            InputController.Instance.OnMouseUp -= PaintObject.OnMouseUp;
            InputController.Instance.OnMouseUp += PaintObject.OnMouseUp;
        }

        protected void UnsubscribeInputEvents()
        {
            InputController.Instance.OnMouseHover -= PaintObject.OnMouseHover;
            InputController.Instance.OnMouseHoverWithHit -= PaintObject.OnMouseHover;
            InputController.Instance.OnMouseDown -= PaintObject.OnMouseDown;
            InputController.Instance.OnMouseDownWithHit -= PaintObject.OnMouseDown;
            InputController.Instance.OnMouseButton -= PaintObject.OnMouseButton;
            InputController.Instance.OnMouseButtonWithHit -= PaintObject.OnMouseButton;
            InputController.Instance.OnMouseUp -= PaintObject.OnMouseUp;
        }

        private void UpdatePreviewInput()
        {
            if (Preview)
            {
                if (_renderComponentsHelper.IsMesh())
                {
                    InputController.Instance.OnMouseHoverWithHit -= PaintObject.OnMouseHover;
                    InputController.Instance.OnMouseHoverWithHit += PaintObject.OnMouseHover;
                }
                else
                {
                    InputController.Instance.OnMouseHover -= PaintObject.OnMouseHover;
                    InputController.Instance.OnMouseHover += PaintObject.OnMouseHover;
                }
            }
            else
            {
                if (_renderComponentsHelper.IsMesh())
                {
                    InputController.Instance.OnMouseHoverWithHit -= PaintObject.OnMouseHover;
                }
                else
                {
                    InputController.Instance.OnMouseHover -= PaintObject.OnMouseHover;
                }
            }
        }
    }
}