using Module.Core.Content;
using UnityEngine;

namespace Module.Engine.ColorPicker.Content {
    public class ContentColorPickerController : ContentControllerWithViewBase<ContentColorPickerView> {
        private readonly Color _defaultModelBackgroundColor = Color.white;
        private Color _currentModelBackgroundColor;

        public void Reset() {
            _currentModelBackgroundColor = _defaultModelBackgroundColor;
            Refresh();
        }

        public void Refresh() {
            View.ColorPicker.SetColor(_currentModelBackgroundColor);
        }

        protected override void Initialize() {
            base.Initialize();
            View.ColorPicker.AddOnSetColorListener(ChangeColor);
        }

        protected override void Release() {
            View.ColorPicker.RemoveOnSetColorListener(ChangeColor);
            base.Release();
        }

        private void ChangeColor(Color color) {
            _currentModelBackgroundColor = color;
        }
    }
}
