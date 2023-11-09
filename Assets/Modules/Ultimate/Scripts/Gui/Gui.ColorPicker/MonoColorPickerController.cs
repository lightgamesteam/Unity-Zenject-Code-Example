using Gui.Core;
using TDL.Modules.Ultimate.GuiBackground;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Ultimate.GuiColorPicker {
    public class MonoColorPickerController : MonoViewControllerBase<MonoColorPickerView> {
        [Inject] private readonly GuiBackgroundController _guiBackgroundController = default;
        [Inject(Id = "Background")] 
        private readonly Camera _backgroundCamera = default;

        private readonly Color _defaultModelBackgroundColor = Color.white;
        private Color _currentModelBackgroundColor;

        public void Reset() {
            _currentModelBackgroundColor = _defaultModelBackgroundColor;
            Refresh();
        }

        public void Refresh() {
            View.ColorPicker.SetColor(_currentModelBackgroundColor);
        }

        public override void Initialize() {
            base.Initialize();
            View.ColorPicker.AddOnSetColorListener(ChangeColor);
            View.ColorPicker.AddOnSetColorListener(ChangeCameraColor);
            View.ColorPicker.AddOnSetActiveListener(value => {
                _guiBackgroundController.SetActiveBackground(!value);
            });
        }

        public override void Dispose() {
            View.ColorPicker.RemoveOnSetColorListener(ChangeColor);
            View.ColorPicker.RemoveOnSetColorListener(ChangeCameraColor);
            base.Dispose();
        }

        private void ChangeColor(Color color) {
            _currentModelBackgroundColor = color;
        }

        private void ChangeCameraColor(Color color) {
            _backgroundCamera.backgroundColor = color;
        }
    }
}