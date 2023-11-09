using Gui.Core;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.GuiControlElements {
    public class MonoControlElementsMobileController : MonoViewControllerBase<MonoControlElementsMobileView> {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;

        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);
        }

        private void Localization(ILanguageHandler languageHandler) {
            if (View.ResetText != null) {
                View.ResetText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.DefaultPositionKey);
            }
            if (View.ColorPickerText != null) {
                View.ColorPickerText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.BackgroundColorKey);
            }
            if (View.LabelsText != null) {
                View.LabelsText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.LabelsKey);
            }
            if (View.LayersText != null) {
                View.LayersText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.LayersKey);
            }
            if (View.FullscreenHideInterfaceText != null) {
                View.FullscreenHideInterfaceText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.HideInterfaceKey);
            }
            if (View.FullscreenShowInterfaceText != null) {
                View.FullscreenShowInterfaceText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.ShowInterfaceKey);
            }
        }
    }
}