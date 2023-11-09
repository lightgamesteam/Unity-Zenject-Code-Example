using System;
using Module.Core.Component;
using UnityEngine;

namespace Module.Engine.ColorPicker.Component {
    public class ComponentColorPickerSimpleController : ComponentControllerBase<ComponentColorPickerSimpleView> {
        public void Reset() {
            View.Content.Controller.Reset();
        }

        public void Refresh() {
            View.Content.Controller.Refresh();
        }

        public void AddOnSetColorListener(Action<Color> color) {
            View.Content.View.ColorPicker.AddOnSetColorListener(color);
        }

        public void RemoveOnSetColorListener(Action<Color> color) {
            View.Content.View.ColorPicker.RemoveOnSetColorListener(color);
        }
    }
}
