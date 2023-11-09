using System;
using TMPro;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class MonoControlElementsPanelInformationalView : Gui.Core.ViewBase {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public TextMeshProUGUI Text;
    }
}