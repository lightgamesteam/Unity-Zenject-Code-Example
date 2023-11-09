using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiTooltip;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiTooltipHelper : TooltipEvents {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        [Inject] private readonly GuiTooltipController _tooltipController = default;

        protected override void Awake() {
            base.Awake();
            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);
        }

        protected override TooltipPanel GetTooltipPanel() {
            return _tooltipController.TooltipPanel;
        }

        private void Localization(ILanguageHandler languageHandler) {
            SetHint(languageHandler.GetCurrentTranslations(GetKey()));
        }
    }
}