using System;
using TDL.Constants;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class ColorPicker : ViewBase
    {
        [SerializeField] private Camera _camera;

        public bool isScreenSpace;
        public Color awakeColor = Color.white;

        [Serializable]
        public class ColorChangeEvent : UnityEvent<Color>
        {
        }

        [Serializable]
        public class ActivationChangeEvent : UnityEvent<bool>
        {
        }

        public ColorChangeEvent onValueChanged = new ColorChangeEvent();
        public ActivationChangeEvent ActivationChanged;
        public Toggle ActivatedToggle { get; private set; }
        public bool IsInitialized { get; private set; }

        private Color _color;
        public Color colorValue => IsInitialized ? _color : awakeColor;
        private Action _colorPickerUpdater;

        private GameObject _saturation;
        private GameObject _saturationKnob;
        private RectTransform _saturationRectTransform;
        private Color[] _saturationColors;
        private Texture2D _saturationTexture;
        private Vector2 _saturationSize;
        private float _saturationValue;

        private GameObject _hue;
        private GameObject _hueKnob;
        private RectTransform _hueRectTransform;
        private Texture2D _hueTexture;
        private Color[] _hueColors;
        private Vector2 _hueSize;
        private float _hueValue;
        private float _hsvValue;

        private const int CameraLayerUI = 5;

        public override void InitUiComponents()
        {
            IsInitialized = true;
            CacheLinks();
            CreateColorPickerPalette();
            Setup(awakeColor);
        }

        private void CacheLinks()
        {
            if (_camera == null)
            {
                _camera = gameObject.scene.name.Equals(SceneNameConstants.Module3DModel) 
                    ? FindComponentExtension.GetInSceneOnLayer<Camera>(CameraLayerUI, SceneNameConstants.Module3DModel) 
                    : Camera.main;
            }

            ActivatedToggle = GetComponentInChildren<Toggle>(true);
        }

        private void OnDestroy()
        {
            IsInitialized = false;
            
            if(_saturationTexture != null)
                DestroyImmediate(_saturationTexture);
            
            if(_hueTexture != null)
                DestroyImmediate(_hueTexture);
        }

        private void CreateColorPickerPalette()
        {
            CreateSaturation();
            CreateHue();
        }

        private void CreateSaturation()
        {
            _saturation = GO("SaturationValue");
            _saturationKnob = GO("SaturationValue/Knob");
            _saturationRectTransform = _saturation.GetComponent<RectTransform>();

            _saturationColors = new[]
            {
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(1, 1, 1),
                new Color(1, 0, 0)
            };

            _saturationTexture = new Texture2D(2, 2);
            _saturation.GetComponent<Image>().sprite = Sprite.Create(_saturationTexture, new Rect(0.5f, 0.5f, 1, 1),
                new Vector2(0.5f, 0.5f));
            _saturationSize = GetWidgetSize(_saturationRectTransform);
        }

        private void CreateHue()
        {
            _hue = GO("Hue");
            _hueKnob = GO("Hue/Knob");
            _hueRectTransform = _hue.GetComponent<RectTransform>();

            _hueColors = new[]
            {
                Color.red,
                Color.yellow,
                Color.green,
                Color.cyan,
                Color.blue,
                Color.magenta
            };

            _hueTexture = new Texture2D(1, _hueColors.Length + 1);
            for (var i = 0; i < _hueColors.Length + 1; i++)
            {
                _hueTexture.SetPixel(0, i, _hueColors[i % _hueColors.Length]);
            }

            _hueTexture.Apply();

            _hue.GetComponent<Image>().sprite =
                Sprite.Create(_hueTexture, new Rect(0, 0.5f, 1, 6), new Vector2(0.5f, 0.5f));
            _hueSize = GetWidgetSize(_hueRectTransform);
        }

        public override void SubscribeOnListeners()
        {
            ActivatedToggle.onValueChanged.AddListener(OnActivated);
        }

        public override void UnsubscribeFromListeners()
        {
            ActivatedToggle.onValueChanged.RemoveAllListeners();
        }

        private void OnActivated(bool isActivated)
        {
            ActivationChanged?.Invoke(isActivated);

            if (isActivated)
            {
                onValueChanged?.Invoke(_color);
            }
            else
            {
                _camera.backgroundColor = Color.white;
                ResetColor(Color.white);
            }
        }

        void Update()
        {
            _colorPickerUpdater();
        }

        private void RGBToHSV(Color color, out float h, out float s, out float v)
        {
            var cmin = Mathf.Min(color.r, color.g, color.b);
            var cmax = Mathf.Max(color.r, color.g, color.b);
            var d = cmax - cmin;
            if (d == 0)
            {
                h = 0;
            }
            else if (cmax == color.r)
            {
                h = Mathf.Repeat((color.g - color.b) / d, 6);
            }
            else if (cmax == color.g)
            {
                h = (color.b - color.r) / d + 2;
            }
            else
            {
                h = (color.r - color.g) / d + 4;
            }

            s = cmax == 0 ? 0 : d / cmax;
            v = cmax;
        }

        private bool GetLocalMouse(RectTransform elementTransform, out Vector2 result)
        {
            var scaleFactor = elementTransform.GetMyCanvas().scaleFactor;
            var mp = elementTransform.InverseTransformPoint(Input.mousePosition);

            if (isScreenSpace)
            {
                var rt2 = (RectTransform) transform;

                mp = MouseExtension.GetMousePositionViewport() / scaleFactor
                     - new Vector3(rt2.anchoredPosition.x + elementTransform.anchoredPosition.x,
                         rt2.anchoredPosition.y + elementTransform.anchoredPosition.y, 0);
            }

            result.x = Mathf.Clamp(mp.x, elementTransform.rect.min.x, elementTransform.rect.max.x);
            result.y = Mathf.Clamp(mp.y, elementTransform.rect.min.y, elementTransform.rect.max.y);

            return RectTransformUtility.RectangleContainsScreenPoint(elementTransform, Input.mousePosition, _camera);
        }

        private Vector2 GetWidgetSize(RectTransform widget)
        {
            return widget.rect.size;
        }

        private GameObject GO(string name)
        {
            return transform.Find(name).gameObject;
        }

        public void ResetColorToDefault()
        {
            ResetColor(Color.white);
        }

        public void ResetColor(Color defaultColor)
        {
            if (IsInitialized)
            {
                Setup(defaultColor);
            }
        }

        private void ResetSaturationTexture()
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    _saturationTexture.SetPixel(i, j, _saturationColors[i + j * 2]);
                }
            }

            _saturationTexture.Apply();
        }

        private void ApplySaturation()
        {
            var sv = new Vector2(_saturationValue, _hsvValue);
            var isv = new Vector2(1 - sv.x, 1 - sv.y);
            var c0 = isv.x * isv.y * _saturationColors[0];
            var c1 = sv.x * isv.y * _saturationColors[1];
            var c2 = isv.x * sv.y * _saturationColors[2];
            var c3 = sv.x * sv.y * _saturationColors[3];
            var resultColor = c0 + c1 + c2 + c3;

            if (_color != resultColor)
            {
                if (ActivatedToggle.isOn)
                {
                    onValueChanged.Invoke(resultColor);
                }

                _color = resultColor;
            }
        }

        private void ApplyHue()
        {
            var i0 = Mathf.Clamp((int) _hueValue, 0, 5);
            var i1 = (i0 + 1) % 6;
            var resultColor = Color.Lerp(_hueColors[i0], _hueColors[i1], _hueValue - i0);
            _saturationColors[3] = resultColor;
        }

        private void SetSaturationKnobPosition()
        {
            _saturationKnob.transform.localPosition =
                new Vector2(_saturationValue * _saturationSize.x, _hsvValue * _saturationSize.y);
        }

        private void SetHueKnobPosition()
        {
            _hueKnob.transform.localPosition =
                new Vector2(_hueKnob.transform.localPosition.x, _hueValue / 6 * _saturationSize.y);
        }

        private void StateIdle()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mp;
                if (GetLocalMouse(_hueRectTransform, out mp))
                {
                    _colorPickerUpdater = StateDragHue;
                }
                else if (GetLocalMouse(_saturationRectTransform, out mp))
                {
                    _colorPickerUpdater = StateDragSaturation;
                }
            }
        }

        private void StateDragSaturation()
        {
            Vector2 mp;
            GetLocalMouse(_saturationRectTransform, out mp);
            _saturationValue = mp.x / _saturationSize.x;
            _hsvValue = mp.y / _saturationSize.y;

            ApplySaturation();

            _saturationKnob.transform.localPosition = mp;
            if (Input.GetMouseButtonUp(0))
            {
                _colorPickerUpdater = StateIdle;
            }
        }

        private void StateDragHue()
        {
            Vector2 mp;
            GetLocalMouse(_hueRectTransform, out mp);
            _hueValue = mp.y / _hueSize.y * 6;

            ApplyHue();
            ResetSaturationTexture();
            ApplySaturation();

            _hueKnob.transform.localPosition = new Vector2(_hueKnob.transform.localPosition.x, mp.y);
            if (Input.GetMouseButtonUp(0))
            {
                _colorPickerUpdater = StateIdle;
            }
        }

        private void Setup(Color inputColor)
        {
            RGBToHSV(inputColor, out _hueValue, out _saturationValue, out _hsvValue);

            ApplyHue();
            ResetSaturationTexture();
            ApplySaturation();

            SetSaturationKnobPosition();
            SetHueKnobPosition();

            _colorPickerUpdater = StateIdle;
        }
    }
}