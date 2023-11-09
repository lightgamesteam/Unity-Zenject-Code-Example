using System.Collections.Generic;
using System.Linq;
using Gui.Core;
using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiScrollbar;
using TDL.Server;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbarLanguages {
    public class MonoScrollbarLanguagesController : MonoViewControllerBase<MonoScrollbarLanguagesView> {
        [Inject] private readonly ILanguageHandler _managerLanguageHandler = default;
        [Inject] private readonly GuiItemPrefabs _guiItemPrefabs = default;
        
        private List<ItemScrollbarLanguagesController> _languageControllerArray;
        
        public override void Initialize() {
            base.Initialize();
            ClearContent();

            _languageControllerArray = new List<ItemScrollbarLanguagesController>();
            foreach (var languageResource in _managerLanguageHandler.GetLanguageResources()) {
                var model = CreateItem(languageResource, _managerLanguageHandler.GetCurrentCulture());
                var component = InstantiatePrefab<ItemScrollbarLanguagesController>(_guiItemPrefabs.ItemScrollbarLanguages, View.ScrollContent);
                component.Initialize(model);
                _languageControllerArray.Add(component);
            }
            SortContent();
        }
        
        #region Private methods
        
        private void ClearContent() {
            if (View.ScrollContent.childCount != 0) {
                for (var i = View.ScrollContent.childCount - 1; i >= 0; i--) {
                    Destroy(View.ScrollContent.GetChild(i).gameObject);
                }
            }
        }

        private static ItemScrollbarLanguagesModel CreateItem(LanguageResource language, string currentCultureCode) {
            var itemState = ItemStateType.Enable;
            var selectState = language.Culture.Equals(currentCultureCode) ? SelectStateType.Active : SelectStateType.None;
            return new ItemScrollbarLanguagesModel { Data = language, ItemStateType = itemState, SelectStateType = selectState, DisplayLabel = language.Name };
        }
        
        private void SortContent() {
            var sorted = SortAlphabetically(_languageControllerArray);
            for (var index = 0; index < sorted.Length; index++) {
                sorted[index].transform.SetSiblingIndex(index);
            }
        }
        
        private static ItemScrollbarLanguagesController[] SortAlphabetically(IEnumerable<ItemScrollbarLanguagesController> items) {
            return items.OrderBy(x => x.Model.Data.Name).ToArray();
        }

        #endregion
    }
}