using TDL.Modules.Ultimate.Signal;

namespace TDL.Modules.Ultimate.Core {
    public class GuiToggleHandlerControlElementsFullscreen : GuiToggleHandler {

        protected override void SendSignal(bool isOn) {
            SignalBus.Fire(new FullscreenCommandSignal(isOn));
        }
    }
}