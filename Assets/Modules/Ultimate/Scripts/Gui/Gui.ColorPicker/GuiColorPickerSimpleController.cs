using Gui.Core;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.GuiColorPicker {
    public class GuiColorPickerSimpleController : GuiViewControllerBase<GuiColorPickerSimpleView> {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;

        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);
        }

        private void Localization(ILanguageHandler languageHandler) {
            if (View.Content.View.TitleText != null) {
                View.Content.View.TitleText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.BackgroundColorKey);
            }
            if (View.Content.View.CloseText != null) {
                View.Content.View.CloseText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.CloseKey);
            }
            View.Content.View.ColorPicker.SetActiveText(languageHandler.GetCurrentTranslations(LocalizationConstants.ActivateKey));
        }
    }
}