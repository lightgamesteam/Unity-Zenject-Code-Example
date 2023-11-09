using System;
using System.Collections.Generic;
using Module.Core.Component;
using Module.Engine.ScrollbarLayers.Item;

namespace Module.Engine.ScrollbarLayers.Component {
    public class ComponentScrollbarLayersSimpleController : ComponentControllerBase<ComponentScrollbarLayersSimpleView> {
        #region Public methods

        public void Initialize(string componentPrefabPath, string layerPrefabPath, string partPrefabPath, ItemScrollbarLayersData[] layerDataArray, Action<Guid> selectLayerAction) {
            View.Content.Controller.Initialize(componentPrefabPath, layerPrefabPath, partPrefabPath, layerDataArray, selectLayerAction);
        }

        public void SelectStateTypeOfNone(Guid guid) { SetState(guid, StateType.None); }
        public void SelectStateTypeOfActive(Guid guid) { SetState(guid, StateType.Active); }

        public void SetState(Guid guid, StateType stateType) {
            View.Content.Controller.SetState(guid, stateType);
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
