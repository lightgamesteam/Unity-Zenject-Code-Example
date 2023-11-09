using System;
using System.Collections.Generic;
using Module.Core.Component;
using Module.Engine.ScrollbarLanguages.Item;

namespace Module.Engine.ScrollbarLanguages.Component {
    public class ComponentScrollbarLanguagesSimpleController : ComponentControllerBase<ComponentScrollbarLanguagesSimpleView> {
        #region Public methods

        public void Initialize(string prefabPath, IEnumerable<ItemScrollbarLanguagesData> dataArray, Action<int> selectAction) {
            View.Content.Controller.Initialize(prefabPath, dataArray, selectAction);
        }

        public void SelectStateTypeOfNone(int id) { SetState(id, ItemStateType.Disable); }
        public void SelectStateTypeOfActive(int id) { SetState(id, ItemStateType.Enable); }

        public void SetState(int id, ItemStateType itemStateType) {
            View.Content.Controller.SetState(id, itemStateType);
        }

        public void RefreshState() {
            View.Content.Controller.RefreshState();
        }

        public void Dispose() {
            View.Content.Controller.Dispose();
        }

        public void Localize(Dictionary<string, string> systemTranslations, string titleKey) {
            View.Content.Controller.Localize(systemTranslations, titleKey);
        }

        #endregion
    }
}
