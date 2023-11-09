using System;
using Module.Core.UIComponent;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class MonoControlElementsPanelApplicationView : Gui.Core.ViewBase {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public ComponentButton CloseButton;
        [SerializeField] public ComponentButton ScreenshotButton;
        [SerializeField] public ComponentToggle FullscreenToggle;
        [SerializeField] public ComponentToggle VideoRecordingToggle;
        [SerializeField] public ComponentButton VideoHelpButton;
    }
}