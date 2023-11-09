using Gui.Core;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbarLanguages {
    public class GuiScrollbarLanguagesSimpleController : GuiViewControllerBase<GuiScrollbarLanguagesSimpleView> {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;

        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);
        }

        private void Localization(ILanguageHandler languageHandler) {
            View.Content.View.TitleText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.SelectLanguageKey);
            if (View.Content.View.CloseText != null) {
                View.Content.View.CloseText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.CloseKey);
            }
        }
    }
}