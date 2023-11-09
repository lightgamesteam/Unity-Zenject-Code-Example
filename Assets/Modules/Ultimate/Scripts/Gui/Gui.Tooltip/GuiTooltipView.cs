using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiTooltip {
    [Serializable]
    public class GuiTooltipView : Gui.Core.ViewBase {
        [SerializeField] public TooltipPanel TooltipPanel;
    }
}