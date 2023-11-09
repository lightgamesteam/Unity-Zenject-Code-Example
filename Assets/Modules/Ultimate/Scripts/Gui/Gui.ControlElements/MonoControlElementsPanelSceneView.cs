using System;
using Module.Core.UIComponent;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class MonoControlElementsPanelSceneView : Gui.Core.ViewBase {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public ComponentButtonTriggers ZoomPlusButton;
        [SerializeField] public ComponentButtonTriggers ZoomMinusButton;
        [SerializeField] public ComponentToggle ColorPickerToggle;
        [SerializeField] public ComponentButton ResetButton;
    }
}