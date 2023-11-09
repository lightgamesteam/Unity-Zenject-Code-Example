using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiScrollbar;

namespace TDL.Modules.Ultimate.GuiScrollbarLanguages {
    public class ItemScrollbarLanguagesController : ItemScrollbarControllerBase<ItemScrollbarLanguagesModel, ItemScrollbarLanguagesView> {
        public override void Initialize(ItemScrollbarModelBase model) {
            base.Initialize(model);
            View.SelectButton.GetComponent<GuiButtonHandlerScrollbarLanguagesItem>().Initialize(Model.Data.Id);
        }

        protected override void ChangeOfLanguage(ILanguageHandler languageHandler) {
            base.ChangeOfLanguage(languageHandler);
            Model.SelectStateType = Model.Data.Culture.Equals(languageHandler.GetCurrentCulture()) ? SelectStateType.Active : SelectStateType.None;
            Model.DisplayLabel = Model.Data.Name;
            RefreshView();
        }
    }
}