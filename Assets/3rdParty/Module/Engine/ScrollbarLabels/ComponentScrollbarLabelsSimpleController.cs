using System;
using System.Collections.Generic;
using Module.Core.Component;
using Module.Engine.ScrollbarLabels.Item;

namespace Module.Engine.ScrollbarLabels.Component {
    public class ComponentScrollbarLabelsSimpleController : ComponentControllerBase<ComponentScrollbarLabelsSimpleView> {
        #region Public methods

        public void Initialize(string componentPrefabPath, string groupPrefabPath, string partPrefabPath, ItemScrollbarLabelsContentData[] contentDataArray, Action<Guid> selectAction) {
            View.Content.Controller.Initialize(componentPrefabPath, groupPrefabPath, partPrefabPath, contentDataArray, selectAction);
        }

        public void SelectStateTypeOfNone(Guid guid) { SetState(guid, StateType.None); }
        public void SelectStateTypeOfActive(Guid guid) { SetState(guid, StateType.Active); }

        public void SetState(Guid guid, StateType stateType) {
            View.Content.Controller.SetState(guid, stateType);
        }

        public void SetBlockState(Guid guid, BlockStateType stateType) {
            View.Content.Controller.SetBlockState(guid, stateType);
        }

        public void FocusToItem(Guid guid) {
            View.Content.Controller.FocusToItem(guid);
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
