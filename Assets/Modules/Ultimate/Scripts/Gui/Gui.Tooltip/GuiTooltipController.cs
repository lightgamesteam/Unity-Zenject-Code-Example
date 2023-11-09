using Gui.Core;

namespace TDL.Modules.Ultimate.GuiTooltip {
    public class GuiTooltipController : GuiViewControllerBase<GuiTooltipView> {
        public TooltipPanel TooltipPanel => View.TooltipPanel;
    }
}