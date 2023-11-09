using Gui.Core;
using Module.Common;
using Module.Core.UIComponent;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiColorPicker;
using TDL.Modules.Ultimate.GuiScrollbarLabels;
using TDL.Modules.Ultimate.GuiScrollbarLanguages;
using TDL.Modules.Ultimate.GuiScrollbarLayers;
using TDL.Signals;
using Zenject;

namespace TDL.Modules.Ultimate.GuiControlElements {
    public class MonoControlElementsController : MonoViewControllerBase<MonoControlElementsView> {
        [Inject] private readonly ModuleEntryPoint _moduleEntryPoint = default;
        [Inject] private readonly ManagerActivityData _activityData = default;
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        [Inject] private readonly GuiScrollbarLanguagesSimpleController _guiScrollbarLanguages = default;
        [Inject] private readonly GuiScrollbarLabelsSimpleController _guiScrollbarLabels = default;
        [Inject] private readonly GuiScrollbarLayersSimpleController _guiScrollbarLayers = default;
        [Inject] private readonly GuiColorPickerSimpleController _guiColorPicker = default;
        [Inject] private readonly SignalBus _signal;
        
        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);
            
            View.PanelApplication.View.CloseButton.AddListener(_moduleEntryPoint.CloseModule);
            View.PanelApplication.View.FullscreenToggle.AddListener(FullscreenAction);
            View.PanelApplication.View.ScreenshotButton.AddListener(TakeScreenshot);
            View.PanelApplication.View.VideoRecordingToggle.AddListener(StartVideoRecording);
            View.PanelApplication.View.VideoHelpButton.AddListener(OpenUrlVideoHelp);
            _signal.Subscribe<VideoRecordingStateSignal>(OnChangeRecordingState);
            View.PanelScene.View.ColorPickerToggle.AddListener(_guiColorPicker.SetStateComponent);
            View.PanelScene.View.ColorPickerToggle.IsOn = false;
            View.PanelScene.View.ColorPickerToggle.Invoke();
            View.PanelAdditional.View.LanguagesToggle.AddListener(ToggleScrollbarLanguages);
            View.PanelAdditional.View.LanguagesToggle.IsOn = false;
            View.PanelAdditional.View.LanguagesToggle.Invoke();
            View.PanelAdditional.View.LabelsToggle.AddListener(ToggleScrollbarLabels);
            View.PanelAdditional.View.LabelsToggle.IsOn = false;
            View.PanelAdditional.View.LabelsToggle.Invoke();
            View.PanelAdditional.View.LayersToggle.AddListener(ToggleScrollbarLayers);
            View.PanelAdditional.View.LayersToggle.IsOn = false;
            View.PanelAdditional.View.LayersToggle.Invoke();
        }
        
        public override void Dispose() 
        {
            base.Dispose();
            
            _signal.Unsubscribe<VideoRecordingStateSignal>(OnChangeRecordingState);
        }

        private void Localization(ILanguageHandler languageHandler) {
            View.PanelInformational.View.Text.text = languageHandler.GetCurrentTranslations(_activityData.ActivityLocal);
        }
        
        private void TakeScreenshot() 
        {
            _signal.Fire(new TakeScreenshotSignal(1));
        }

        private void StartVideoRecording(bool isActive)
        {
            _signal.Fire(new StartVideoRecordingSignal(isActive, 1, View.PanelApplication.View.VideoRecordingToggle.GetToggle()));
        }

        private void OpenUrlVideoHelp()
        {
            _signal.Fire(new OpenUrlCommandSignal(ServerConstants.HelpVideoRecordingUrl));
        }

        private void OnChangeRecordingState(VideoRecordingStateSignal signal)
        {
            if (signal.State == RecordingState.StartRecording)
            {
                View.PanelApplication.View.CloseButton.Interactable = false;
            }
            else
            {
                View.PanelApplication.View.CloseButton.Interactable = true;
            }
        }

        private void FullscreenAction(bool isActive) {
            View.PanelScene.View.CanvasGroup.SetActive(!isActive);
            View.PanelAdditional.View.CanvasGroup.SetActive(!isActive);
            View.PanelInformational.View.CanvasGroup.SetActive(!isActive);

            if (isActive) {
                _guiScrollbarLanguages.HideComponent();
                _guiScrollbarLabels.HideComponent();
                _guiScrollbarLayers.HideComponent();
                _guiColorPicker.HideComponent();
            } else {
                _guiScrollbarLanguages.SetStateComponent(View.PanelAdditional.View.LanguagesToggle.IsOn);
                _guiScrollbarLabels.SetStateComponent(View.PanelAdditional.View.LabelsToggle.IsOn);
                _guiScrollbarLayers.SetStateComponent(View.PanelAdditional.View.LayersToggle.IsOn);
                _guiColorPicker.SetStateComponent(View.PanelScene.View.ColorPickerToggle.IsOn);
            }
        }

        private void ToggleScrollbarLanguages(bool isOn) {
            SetAdditionalPanel(View.PanelAdditional.View.LanguagesToggle, _guiScrollbarLanguages, isOn);
        }
        
        private void ToggleScrollbarLabels(bool isOn) {
            SetAdditionalPanel(View.PanelAdditional.View.LabelsToggle, _guiScrollbarLabels, isOn);
        }
        
        private void ToggleScrollbarLayers(bool isOn) {
            SetAdditionalPanel(View.PanelAdditional.View.LayersToggle, _guiScrollbarLayers, isOn);
        }

        private void SetAdditionalPanel(ComponentToggle componentToggle, MonoControllerBase controllerBase, bool isOn) {
            if (isOn) {
                HideAdditionalPanelWhenNoEquals(componentToggle, View.PanelAdditional.View.LanguagesToggle);
                HideAdditionalPanelWhenNoEquals(componentToggle, View.PanelAdditional.View.LabelsToggle);
                HideAdditionalPanelWhenNoEquals(componentToggle, View.PanelAdditional.View.LayersToggle);
            }
            controllerBase.SetStateComponent(isOn);
        }

        private static void HideAdditionalPanelWhenNoEquals(ComponentToggle componentToggle, ComponentToggle hideComponentToggle) {
            if (!componentToggle.Equals(hideComponentToggle)) {
                hideComponentToggle.IsOn = false;
            }
        }
    }
}