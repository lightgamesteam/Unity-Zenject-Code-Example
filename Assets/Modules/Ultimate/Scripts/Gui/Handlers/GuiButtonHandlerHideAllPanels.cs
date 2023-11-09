using TDL.Modules.Ultimate.Signal;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonHandlerHideAllPanels : GuiButtonHandler {
        protected override void SendSignal() {
            SignalBus.Fire(new ControlElementsHideAllPanelsCommandSignal());
        }
    }
}