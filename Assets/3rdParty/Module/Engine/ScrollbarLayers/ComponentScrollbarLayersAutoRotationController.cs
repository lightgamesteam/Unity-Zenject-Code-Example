using System;
using System.Collections.Generic;
using Module.Core.Component;
using Module.Engine.ScrollbarLayers.Item;

namespace Module.Engine.ScrollbarLayers.Component {
    public class ComponentScrollbarLayersAutoRotationController : ComponentControllerBase<ComponentScrollbarLayersAutoRotationView> {
        #region Public methods

        public void Initialize(string componentPrefabPath, string layerPrefabPath, string partPrefabPath, ItemScrollbarLayersData[] layerDataArray, Action<Guid> selectLayerAction) {
            View.Landscape.Controller.Initialize(componentPrefabPath, layerPrefabPath, partPrefabPath, layerDataArray, selectLayerAction);
            View.Portrait.Controller.Initialize(componentPrefabPath, layerPrefabPath, partPrefabPath, layerDataArray, selectLayerAction);
        }

        public void SelectStateTypeOfNone(Guid guid) { SetState(guid, StateType.None); }
        public void SelectStateTypeOfActive(Guid guid) { SetState(guid, StateType.Active); }

        public void SetState(Guid guid, StateType stateType) {
            View.Landscape.Controller.SetState(guid, stateType);
            View.Portrait.Controller.SetState(guid, stateType);
        }

        public void FocusToItem(Guid guid) {
            View.Landscape.Controller.FocusToItem(guid);
            View.Portrait.Controller.FocusToItem(guid);
        }

        public void RefreshState() {
            View.Landscape.Controller.RefreshState();
            View.Portrait.Controller.RefreshState();
        }

        public void Dispose() {
            View.Landscape.Controller.Dispose();
            View.Portrait.Controller.Dispose();
        }

        public void Localize(Dictionary<string, string> systemTranslations, string titleKey) {
            View.Landscape.Controller.Localize(systemTranslations, titleKey);
            View.Portrait.Controller.Localize(systemTranslations, titleKey);
        }

        #endregion
    }
}
