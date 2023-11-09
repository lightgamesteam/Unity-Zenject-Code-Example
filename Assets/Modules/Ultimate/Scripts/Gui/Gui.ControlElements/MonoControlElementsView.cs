using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class MonoControlElementsView : Gui.Core.ViewBase {
        [SerializeField] public MonoControlElementsPanelApplicationController PanelApplication;
        [SerializeField] public MonoControlElementsPanelSceneController PanelScene;
        [SerializeField] public MonoControlElementsPanelAdditionalController PanelAdditional;
        [SerializeField] public MonoControlElementsPanelInformationalController PanelInformational;
    }
}