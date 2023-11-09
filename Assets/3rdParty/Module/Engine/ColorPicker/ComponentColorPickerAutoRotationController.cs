using System;
using Module.Core.Component;
using UnityEngine;

namespace Module.Engine.ColorPicker.Component {
    public class ComponentColorPickerAutoRotationController : ComponentControllerBase<ComponentColorPickerAutoRotationView> {
        public void Reset() {
            View.Landscape.Controller.Reset();
            View.Portrait.Controller.Reset();
        }

        public void Refresh() {
            View.Landscape.Controller.Refresh();
            View.Portrait.Controller.Refresh();
        }

        public void AddOnSetColorListener(Action<Color> color) {
            View.Landscape.View.ColorPicker.AddOnSetColorListener(color);
            View.Portrait.View.ColorPicker.AddOnSetColorListener(color);
        }

        public void RemoveOnSetColorListener(Action<Color> color) {
            View.Landscape.View.ColorPicker.RemoveOnSetColorListener(color);
            View.Portrait.View.ColorPicker.RemoveOnSetColorListener(color);
        }
    }
}
