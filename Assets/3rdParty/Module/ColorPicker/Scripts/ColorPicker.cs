using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module.ColorPicker {
    public class ColorPicker : MonoBehaviour {
        #region Variables

        [SerializeField] protected Color Color = Color.white;

        [SerializeField] protected Image HueImage;
        [SerializeField] protected Image GradientImage;

        [SerializeField] protected ColorJoystick ColorJoystick;

        [SerializeField] protected Slider HueSlider;
        
        protected Texture2D HueTexture;
        protected Texture2D GradientTexture;

        private event Action<Color> OnSetColorEvent;

        #endregion

        #region Unity methods

        protected virtual void Awake() {
            CreateHue();
            CreateGradient();
            SetColor(Color);

            ColorJoystick.AddOnDragListener(OnDragAction);
            HueSlider.onValueChanged.AddListener(OnHueChangedAction);
        }

        private void OnDestroy()
        {
            if(HueTexture != null)
                DestroyImmediate(HueTexture);
        
            if(GradientTexture != null)
                DestroyImmediate(GradientTexture);
        }

        #endregion

        #region Public methods

        public Color GetColor() {
            return Color;
        }

        public void SetColor(Color color) {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            SetColor(h, s, v);
        }

        public void SetColor(float h, float s, float v) {
            var color = Color.HSVToRGB(h, s, v);
            color.a = Color.a;

            Color = color;
            ColorJoystick.SetColorAndPosition(Color, s, v);
            HueSlider.value = h;

            UpdateGradient(HueSlider.value);
            OnSetColor();
        }

        public void AddOnSetColorListener(Action<Color> color) {
            OnSetColorEvent += color;
        }

        public void RemoveOnSetColorListener(Action<Color> color) {
            if (OnSetColorEvent != null) {
                OnSetColorEvent -= color;
            }
        }
        
        #endregion

        private void CreateHue() {
            HueTexture = new Texture2D(1, 128) { filterMode = FilterMode.Point };
            HueImage.sprite = Sprite.Create(HueTexture, new Rect(0f, 0f, HueTexture.width, HueTexture.height), new Vector2(0.5f, 0.5f), 100f);

            var pixels = new List<Color>();
            for (float y = 0; y < HueTexture.height; y++) {
                pixels.Add(Color.HSVToRGB(y / (HueTexture.height - 1), 1f, 1f));
            }
            HueTexture.SetPixels(pixels.ToArray());
            HueTexture.Apply();
        }

        private void CreateGradient() {
            GradientTexture = new Texture2D(128, 128) { filterMode = FilterMode.Point };
            GradientImage.sprite = Sprite.Create(GradientTexture, new Rect(0f, 0f, GradientTexture.width, GradientTexture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private void UpdateGradient(float h) {
            var pixels = new List<Color>();
            for (float y = 0; y < GradientTexture.height; y++) {
                for (float x = 0; x < GradientTexture.width; x++) {
                    pixels.Add(Color.HSVToRGB(h, x / GradientTexture.width, y / GradientTexture.height));
                }
            }
            GradientTexture.SetPixels(pixels.ToArray());
            GradientTexture.Apply();
        }

        private void OnHueChangedAction(float value) {
            float h, s, v;
            Color.RGBToHSV(Color, out h, out s, out v);
            h = value;
            SetColor(h, s, v);
        }

        private void OnDragAction(float x01, float y01) {
            SetColor(HueSlider.value, x01, y01);
        }

        private void OnSetColor() {
            OnSetColorEvent?.Invoke(Color);
        }
    }
}
