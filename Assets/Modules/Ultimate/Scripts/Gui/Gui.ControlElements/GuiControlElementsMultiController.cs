using Gui.Core;
using Gui.Core.UIMulti;
using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.GuiControlElements {
    public class GuiControlElementsMultiController : GuiViewControllerBase<GuiControlElementsMultiView> {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        [Inject] private readonly ManagerActivityData _activityData = default;
        [Inject] private readonly AsyncProcessorService _asyncProcessor = default;
        public MultiButton ResetButton { get; private set; }
        public MultiToggle LanguagesToggles { get; private set; }
        public MultiToggle LabelsToggles { get; private set; }
        public MultiToggle LayersToggles { get; private set; }
        public MultiToggle ColorPickerToggles { get; private set; }
        public MultiToggle FullscreenToggles { get; private set; }

        protected override void ScreenOrientationToLandscape() {
            base.ScreenOrientationToLandscape();
            View.ContentPortrait.HideComponent();
            View.ContentLandscape.ShowComponent();
        }

        protected override void ScreenOrientationToPortrait() {
            base.ScreenOrientationToPortrait();
            View.ContentLandscape.HideComponent();
            View.ContentPortrait.ShowComponent();
        }
        
        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);

            ResetButton = new MultiButton(View.ContentLandscape.View.ResetButton, View.ContentPortrait.View.ResetButton);
            LanguagesToggles = new MultiToggle(View.ContentLandscape.View.LanguagesToggle, View.ContentPortrait.View.LanguagesToggle);
            LabelsToggles = new MultiToggle(View.ContentLandscape.View.LabelsToggle, View.ContentPortrait.View.LabelsToggle);
            LayersToggles = new MultiToggle(View.ContentLandscape.View.LayersToggle, View.ContentPortrait.View.LayersToggle);
            ColorPickerToggles = new MultiToggle(View.ContentLandscape.View.ColorPickerToggle, View.ContentPortrait.View.ColorPickerToggle);
            FullscreenToggles = new MultiToggle(View.ContentLandscape.View.FullscreenToggle, View.ContentPortrait.View.FullscreenToggle);

            _asyncProcessor.Wait(0, () => LanguagesToggles.Invoke(false));
            _asyncProcessor.Wait(0, () => LabelsToggles.Invoke(false));
            _asyncProcessor.Wait(0, () => LayersToggles.Invoke(false));
            _asyncProcessor.Wait(0, () => ColorPickerToggles.Invoke(false));
            _asyncProcessor.Wait(0, () => FullscreenToggles.Invoke(false));
        }

        private void Localization(ILanguageHandler languageHandler) {
            var infoText = languageHandler.GetCurrentTranslations(_activityData.ActivityLocal);
            View.ContentLandscape.View.InformationText.text = infoText;
            View.ContentPortrait.View.InformationText.text = infoText;
            var languageText = languageHandler.GetCurrentName();
            View.ContentLandscape.View.LanguagesText.text = languageText;
            View.ContentPortrait.View.LanguagesText.text = languageText;
        }
    }
}