using TDL.Modules.Ultimate.GuiScrollbarLanguages;
using TDL.Modules.Ultimate.Signal;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiToggleHandlerControlElementsLanguages : GuiToggleHandler {
        [Inject] private readonly GuiScrollbarLanguagesSimpleController _guiScrollbar = default;

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