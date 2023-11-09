using TDL.Modules.Ultimate.GuiColorPicker;
using TDL.Modules.Ultimate.Signal;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiToggleHandlerControlElementsColorPicker : GuiToggleHandler {
        [Inject] private readonly GuiColorPickerSimpleController _guiController = default;

        protected override void SendSignal(bool isOn) {
            if (isOn) {
                SignalBus.Fire(new ControlElementsHideAllPanelsExceptCommandSignal(Toggle));
            } else {
                SignalBus.Fire(new ControlElementsHidePanelCommandSignal(Toggle));
            }
            _guiController.SetStateComponent(isOn);
        }
    }
}