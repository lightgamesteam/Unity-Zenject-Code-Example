using System;
using Module.Core.UIComponent;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class MonoControlElementsPanelAdditionalView : Gui.Core.ViewBase {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public ComponentToggle LanguagesToggle;
        [SerializeField] public ComponentToggle LabelsToggle;
        [SerializeField] public ComponentToggle LayersToggle;
    }
}