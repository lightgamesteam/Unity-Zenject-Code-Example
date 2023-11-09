using TDL.Modules.Ultimate.GuiColorPicker;
using TDL.Modules.Ultimate.GuiControlElements;
using TDL.Modules.Ultimate.GuiScrollbarLabels;
using TDL.Modules.Ultimate.GuiScrollbarLanguages;
using TDL.Modules.Ultimate.GuiScrollbarLayers;
using Zenject;

namespace TDL.Modules.Ultimate.Signal {
    public class FullscreenCommand : ICommandWithParameters {
        [Inject] private readonly GuiControlElementsMultiController _guiControlElements = default;
        [Inject] private readonly GuiScrollbarLanguagesSimpleController _guiScrollbarLanguages = default;
        [Inject] private readonly GuiScrollbarLabelsSimpleController _guiScrollbarLabels = default;
        [Inject] private readonly GuiScrollbarLayersSimpleController _guiScrollbarLayers = default;
        [Inject] private readonly GuiColorPickerSimpleController _guiColorPicker = default;
    
        public void Execute(ISignal signal) {
            var parameter = (FullscreenCommandSignal) signal;
            FullscreenAction(parameter.IsActive);
        }
        
        private void FullscreenAction(bool isActive) {
            _guiControlElements.FullscreenToggles.isOn = isActive;
            
            _guiControlElements.View.ContentLandscape.View.PanelTopCanvasGroup.SetActive(!isActive);
            _guiControlElements.View.ContentPortrait.View.PanelTopCanvasGroup.SetActive(!isActive);
            _guiControlElements.View.ContentLandscape.View.PanelCenterCanvasGroup.SetActive(!isActive);
            _guiControlElements.View.ContentPortrait.View.PanelCenterCanvasGroup.SetActive(!isActive);

            if (isActive) {
                _guiScrollbarLanguages.HideComponent();
                _guiScrollbarLabels.HideComponent();
                _guiScrollbarLayers.HideComponent();
                _guiColorPicker.HideComponent();
            } else {
                _guiScrollbarLanguages.SetStateComponent(_guiControlElements.LanguagesToggles.isOn);
                _guiScrollbarLabels.SetStateComponent(_guiControlElements.LabelsToggles.isOn);
                _guiScrollbarLayers.SetStateComponent(_guiControlElements.LayersToggles.isOn);
                _guiColorPicker.SetStateComponent(_guiControlElements.ColorPickerToggles.isOn);
            }
        }
    }

    public class FullscreenCommandSignal : ISignal {
        public readonly bool IsActive;
    
        public FullscreenCommandSignal(bool isActive) {
            IsActive = isActive;
        }
    }
}