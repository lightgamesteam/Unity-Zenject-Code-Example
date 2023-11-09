using Gui.Core.UIMulti;
using TDL.Modules.Ultimate.GuiControlElements;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Ultimate.Signal {
    public class ControlElementsMultiCommand : ICommandWithParameters {
        [Inject] private readonly GuiControlElementsMultiController _guiControlElements = default;
    
        public void Execute(ISignal signal) {
            switch (signal) {
                case ControlElementsHideAllPanelsCommandSignal _:
                    HideAllPanels(_guiControlElements.LanguagesToggles);
                    HideAllPanels(_guiControlElements.LabelsToggles);
                    HideAllPanels(_guiControlElements.LayersToggles);
                    HideAllPanels(_guiControlElements.ColorPickerToggles);
                    break;
                case ControlElementsHideAllPanelsExceptCommandSignal hideAllPanelsExcept:
                    HideAllPanelsWhenNoEquals(hideAllPanelsExcept.Toggle, _guiControlElements.LanguagesToggles);
                    HideAllPanelsWhenNoEquals(hideAllPanelsExcept.Toggle, _guiControlElements.LabelsToggles);
                    HideAllPanelsWhenNoEquals(hideAllPanelsExcept.Toggle, _guiControlElements.LayersToggles);
                    HideAllPanelsWhenNoEquals(hideAllPanelsExcept.Toggle, _guiControlElements.ColorPickerToggles);
                    break;
                case ControlElementsHidePanelCommandSignal hidePanel:
                    HidePanelsWhenEquals(hidePanel.Toggle, _guiControlElements.LanguagesToggles);
                    HidePanelsWhenEquals(hidePanel.Toggle, _guiControlElements.LabelsToggles);
                    HidePanelsWhenEquals(hidePanel.Toggle, _guiControlElements.LayersToggles);
                    HidePanelsWhenEquals(hidePanel.Toggle, _guiControlElements.ColorPickerToggles);
                    break;
            }
        }
        
        private static void HideAllPanels(MultiToggle hideMultiToggles) {
            hideMultiToggles.isOn = false;
        }
        
        private static void HideAllPanelsWhenNoEquals(Toggle toggle, MultiToggle hideMultiToggles) {
            hideMultiToggles.isOn = hideMultiToggles.Contains(toggle);
        }
        
        private static void HidePanelsWhenEquals(Toggle toggle, MultiToggle hideMultiToggles) {
            if (hideMultiToggles.Contains(toggle)) {
                hideMultiToggles.isOn = false;
            }
        }
    }

    public class ControlElementsHideAllPanelsCommandSignal : ISignal {}
    
    public class ControlElementsHideAllPanelsExceptCommandSignal : ISignal {
        public readonly Toggle Toggle;

        public ControlElementsHideAllPanelsExceptCommandSignal(Toggle toggle) {
            Toggle = toggle;
        }
    }
    
    public class ControlElementsHidePanelCommandSignal : ISignal {
        public readonly Toggle Toggle;

        public ControlElementsHidePanelCommandSignal(Toggle toggle) {
            Toggle = toggle;
        }
    }
}