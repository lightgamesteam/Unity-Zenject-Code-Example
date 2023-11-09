using TDL.Modules.Ultimate.GuiScrollbarLayers;
using TDL.Modules.Ultimate.Signal;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiToggleHandlerControlElementsLayers : GuiToggleHandler {
        [Inject] private readonly GuiScrollbarLayersSimpleController _guiScrollbar = default;

        protected override void SendSignal(bool isOn) {
            if (isOn) {
                SignalBus.Fire(new ControlElementsHideAllPanelsExceptCommandSignal(Toggle));
            } else {
                SignalBus.Fire(new ControlElementsHidePanelCommandSignal(Toggle));
            }
            _guiScrollbar.SetStateComponent(isOn);
        }
    }
}