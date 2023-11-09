using System;
using TMPro;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiColorPicker {
    [Serializable]
    public class MonoColorPickerView : Gui.Core.ViewBase {
        [SerializeField] public Module.ColorPicker.ColorPickerWithActive ColorPicker;
        [Header("Localization")]
        [SerializeField] public TextMeshProUGUI TitleText;
        [SerializeField] public TextMeshProUGUI CloseText;
    }
}