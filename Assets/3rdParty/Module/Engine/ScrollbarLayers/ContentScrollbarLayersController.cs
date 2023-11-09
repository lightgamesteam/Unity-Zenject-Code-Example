using System;
using System.Collections.Generic;
using System.Linq;
using Module.Core;
using Module.Core.Content;
using Module.Engine.ScrollbarLayers.Item;
using UnityEngine;

namespace Module.Engine.ScrollbarLayers.Content {
    public class ContentScrollbarLayersController : ContentControllerWithViewBase<ContentScrollbarLayersView> {
        protected Dictionary<Guid, ItemScrollbarLayersData> IdToLayerData;
        protected Dictionary<Guid, ItemScrollbarLayersPartData> IdToPartData;
        protected Dictionary<ItemScrollbarLayersPartData, ItemScrollbarLayersPartViewBase> DataToController;

        #region Public methods

        public void Initialize(string componentPrefabPath, string layerPrefabPath, string partPrefabPath, ItemScrollbarLayersData[] layerDataArray, Action<Guid> selectLayerAction) {
            Dispose();

            var componentPrefab = Utilities.Resources.GetGameObject(componentPrefabPath);
            var layerPrefab = Utilities.Resources.GetGameObject(layerPrefabPath);
            var partPrefab = Utilities.Resources.GetGameObject(partPrefabPath);
            ConnectDictionary(layerDataArray, out IdToLayerData);
            ConnectDictionary(layerDataArray, out IdToPartData);
            ConnectDictionary(layerDataArray, componentPrefab, layerPrefab, partPrefab, View.ScrollContent, out DataToController);
            InitializeComponent(DataToController, selectLayerAction);
            RefreshState();
        }

        public void SetState(Guid guid, StateType stateType) {
            if (IdToPartData.ContainsKey(guid)) {
                IdToPartData[guid].SetState(stateType);
            }
            RefreshState();
        }

        public void FocusToItem(Guid guid) {
            if (!IdToPartData.ContainsKey(guid)) { return; }

            var component = DataToController[IdToPartData[guid]];
            float normalizePosition;
            if (component.transform.GetSiblingIndex() == 0) {
                normalizePosition = 0;
            } else if (component.transform.GetSiblingIndex() == View.ScrollContent.childCount - 1) {
                normalizePosition = 1;
            } else {
                var cellHalfWidth = 1 / (float) View.ScrollContent.childCount * .5f;
                normalizePosition = Mathf.Clamp01(component.transform.GetSiblingIndex() / (float)View.ScrollContent.childCount + cellHalfWidth);
            }
            View.ScrollRect.verticalNormalizedPosition = 1 - normalizePosition;
            View.ScrollRect.horizontalNormalizedPosition = normalizePosition;
        }

        public void RefreshState() {
            ProcessState();
            foreach (var pair in DataToController) {
                pair.Value.SetLightState(pair.Key.LightStateType);
                if (pair.Value is Item.ItemScrollbarLayersPartView view) {
                    view.SetEyeState(pair.Key.EyeStateType);
                }
            }
        }

        public void Dispose() {
            if (View.ScrollContent.childCount == 0) { return; }

            var count = View.ScrollContent.childCount;
            for (var i = count - 1 ; i >= 0; i--) {
                UnityEngine.Object.Destroy(View.ScrollContent.GetChild(i).gameObject);
            }
        }

        public void Localize(Dictionary<string, string> systemTranslations, string titleKey) {
            View.TitleText.text = systemTranslations[titleKey];
        }

        #endregion

        #region Private methods

        private void ProcessState() {
            foreach (var layerData in IdToLayerData) {
                if (layerData.Value.PartData.StateType == StateType.Active) {
                    layerData.Value.PartData.SetLightState(LightStateType.Enable);
                    foreach (var partData in layerData.Value.SubPartDataArray) {
                        partData.SetLightState(partData.StateType == StateType.Active ? LightStateType.Enable : LightStateType.Disable);
                        partData.SetEyeState(partData.StateType == StateType.Active ? EyeStateType.None : EyeStateType.Hidden);
                    }
                } else {
                    layerData.Value.PartData.SetLightState(LightStateType.Disable);
                    foreach (var partData in layerData.Value.SubPartDataArray) {
                        partData.SetLightState(LightStateType.Disable);
                        partData.SetEyeState(partData.StateType == StateType.Active ? EyeStateType.None : EyeStateType.Hidden);
                    }
                }
            }
        }

        private static void ConnectDictionary(
            IEnumerable<ItemScrollbarLayersData> itemDataArray, 
            out Dictionary<Guid, ItemScrollbarLayersData> dictionary)
        {
            dictionary = itemDataArray.ToDictionary(data => data.Guid);
        }

        private static void ConnectDictionary(
            IEnumerable<ItemScrollbarLayersData> itemDataArray, 
            out Dictionary<Guid, ItemScrollbarLayersPartData> dictionary)
        {
            dictionary = new Dictionary<Guid, ItemScrollbarLayersPartData>();
            foreach (var data in itemDataArray) {
                dictionary.Add(data.PartData.Guid, data.PartData);
                foreach (var subData in data.SubPartDataArray) {
                    dictionary.Add(subData.Guid, subData);
                }
            }
        }

        private static void ConnectDictionary(
            IEnumerable<ItemScrollbarLayersData> itemDataArray,
            GameObject componentPrefab, GameObject layerPrefab, GameObject partPrefab, Transform content,
            out Dictionary<ItemScrollbarLayersPartData, ItemScrollbarLayersPartViewBase> dictionary)
        {
            dictionary = new Dictionary<ItemScrollbarLayersPartData, ItemScrollbarLayersPartViewBase>();
            foreach (var data in itemDataArray) {
                var component = InstantiatePrefab<ItemScrollbarLayersSelectComponentView>(componentPrefab, content);
                component.Initialize(layerPrefab, partPrefab, data.SubPartDataArray.Length);
                dictionary.Add(data.PartData, component.LayerView);
                for (var index = 0; index < data.SubPartDataArray.Length; index++) {
                    dictionary.Add(data.SubPartDataArray[index], component.PartViewArray[index]);
                }
            }
        }

        private static void InitializeComponent(
            Dictionary<ItemScrollbarLayersPartData, ItemScrollbarLayersPartViewBase> dictionary, 
            Action<Guid> selectLayerAction)
        {
            foreach (var pair in dictionary) {
                pair.Value.Initialize(pair.Key.DisplayLabel, () => { selectLayerAction.Invoke(pair.Key.Guid); });
            }
        }

        private static T InstantiatePrefab<T>(GameObject prefab, Transform parent) {
            var view = UnityEngine.Object.Instantiate(prefab);
            view.transform.SetParent(parent);
            view.transform.localScale = Vector3.one;
            return view.GetComponent<T>();
        }

        #endregion
    }
}
