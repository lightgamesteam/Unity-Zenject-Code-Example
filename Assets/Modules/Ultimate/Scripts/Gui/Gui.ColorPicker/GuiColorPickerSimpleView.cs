using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiColorPicker {
    [Serializable]
    public class GuiColorPickerSimpleView : Gui.Core.ViewBase {
        [SerializeField] public MonoColorPickerController Content;
    }
}