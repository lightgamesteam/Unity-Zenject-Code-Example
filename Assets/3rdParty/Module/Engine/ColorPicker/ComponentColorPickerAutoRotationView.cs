using System;
using Module.Engine.ColorPicker.Content;
using UnityEngine;

namespace Module.Engine.ColorPicker.Component {
    [Serializable]
    public class ComponentColorPickerAutoRotationView {
        [SerializeField] public ContentColorPicker Landscape;
        [SerializeField] public ContentColorPicker Portrait;
    }
}
