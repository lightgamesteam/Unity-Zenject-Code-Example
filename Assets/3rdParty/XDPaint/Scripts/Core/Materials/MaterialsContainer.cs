using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XDPaint.Core.Materials
{
    [Serializable]
    public class MaterialsContainer : IMaterialsContainer
    {
        #region Properties and variables
        [SerializeField] private string shaderTextureName = "_MainTex";
        public string ShaderTextureName
        {
            get { return shaderTextureName; }
            set { shaderTextureName = value; }
        }

        public Texture SourceTexture { get; private set; }

        public int SourceTextureWidth
        {
            get { return SourceTexture.width; }
        }

        public int SourceTextureHeight
        {
            get { return SourceTexture.height; }
        }

        [SerializeField] private PaintMode paintMode;
        public PaintMode PaintMode
        {
            get { return paintMode; }
            set
            {
                paintMode = value;
                brush.SetPaintMode(paintMode);
                paint.SetPaintMode(paintMode, _preview, brush.Texture);
            }
        }

        private bool IsMaterialsInitialized
        {
            get { return sourceMaterial != null; }
        }

        private bool _preview;
        public bool Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                brush.SetPaintMode(paintMode);
                paint.SetPaintMode(paintMode, _preview, brush.Texture);
            }
        }
        
        [SerializeField] private Brush brush;
        public Brush Brush
        {
            get { return brush; }
        }
        
        [SerializeField] private Paint paint;
        public Paint Paint
        {
            get { return paint; }
        }

        [SerializeField] private Material sourceMaterial;
        private IRenderComponentsHelper _renderComponentsHelper;
        private Material _objectMaterial;
        private bool _initialized;
        #endregion

        public MaterialsContainer()
        {
            if (brush == null)
            {
                brush = new Brush();
            }
            if (paint == null)
            {
                paint = new Paint();
            }
        }

        public void Init(IRenderComponentsHelper renderComponentsHelper, bool preview)
        {
            _renderComponentsHelper = renderComponentsHelper;
            _preview = preview;
            if (sourceMaterial != null)
            {
                _objectMaterial = Object.Instantiate(sourceMaterial);
            }
            else if (_renderComponentsHelper.Material != null)
            {
                _objectMaterial = Object.Instantiate(_renderComponentsHelper.Material);
            }
            SourceTexture = _renderComponentsHelper.GetSourceTexture(_objectMaterial, shaderTextureName);
            InitBrush();
            InitPaint();
            _initialized = true;
        }

        private void InitBrush()
        {
            brush.onChangeColor += paint.SetColor;
            brush.onChangeTexture += paint.SetTexture;
            brush.Init();
            Brush.SetPaintMode(paintMode);
        }

        private void InitPaint()
        {
            paint.Init(SourceTexture);
            paint.SetPaintMode(paintMode, _preview, brush.Texture);
        }

        public void InitMaterial(Material material)
        {
            if (!IsMaterialsInitialized)
            {
                if (material != null)
                {
                    sourceMaterial = material;
                }
            }
        }

        public void RestoreTexture()
        {
            if (!_initialized)
                return;
            if (SourceTexture != null)
            {
                _objectMaterial.SetTexture(shaderTextureName, SourceTexture);
            }
            else
            {
                _renderComponentsHelper.Material = sourceMaterial;
            }
        }

        public void SetObjectMaterialTexture(Texture texture)
        {
            if (!_initialized)
                return;
            _objectMaterial.SetTexture(shaderTextureName, texture);
            _renderComponentsHelper.SetSourceMaterial(_objectMaterial);
        }
    }
}