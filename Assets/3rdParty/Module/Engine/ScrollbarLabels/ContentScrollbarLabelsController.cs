using System;
using System.Collections.Generic;
using Module.Core;
using Module.Core.Content;
using Module.Engine.ScrollbarLabels.Item;
using UnityEngine;

namespace Module.Engine.ScrollbarLabels.Content {
    public class ContentScrollbarLabelsController : ContentControllerWithViewBase<ContentScrollbarLabelsView> {
        protected Dictionary<Guid, ItemScrollbarLabelsGroupData> IdToGroupData;
        protected Dictionary<Guid, ItemScrollbarLabelsPartData> IdToPartData;
        protected Dictionary<ItemScrollbarLabelsContentData, ItemScrollbarLabelsSelectComponentView> ContentDataToController;
        protected Dictionary<ItemScrollbarLabelsGroupData, ItemScrollbarLabelsGroupView> GroupDataToController;
        protected Dictionary<ItemScrollbarLabelsPartData, ItemScrollbarLabelsPartView> PartDataToController;

        #region Public methods

        public void Initialize(string componentPrefabPath, string groupPrefabPath, string partPrefabPath, ItemScrollbarLabelsContentData[] contentDataArray, Action<Guid> selectAction) {
            Dispose();

            var componentPrefab = Utilities.Resources.GetGameObject(componentPrefabPath);
            var groupPrefab = Utilities.Resources.GetGameObject(groupPrefabPath);
            var partPrefab = Utilities.Resources.GetGameObject(partPrefabPath);
            Connect.Dictionary.DataByGuid(contentDataArray, out IdToGroupData);
            Connect.Dictionary.DataByGuid(contentDataArray, out IdToPartData);
            Connect.Dictionary.ControllerByData(contentDataArray, componentPrefab, groupPrefab, partPrefab, View.ScrollContent, out ContentDataToController, out GroupDataToController, out PartDataToController);
            Connect.Initialize.Component(GroupDataToController);
            Connect.Initialize.Component(PartDataToController, selectAction);
            RefreshState();
        }

        public void SetState(Guid guid, StateType stateType) {
            if (IdToPartData.ContainsKey(guid)) {
                IdToPartData[guid].SetState(stateType);
            }
            RefreshState();
        }

        public void SetBlockState(Guid guid, BlockStateType stateType) {
            if (IdToPartData.ContainsKey(guid)) {
                IdToPartData[guid].SetBlockState(stateType);
            }
            RefreshState();
        }

        public void FocusToItem(Guid guid) {
            if (!IdToPartData.ContainsKey(guid)) { return; }

            var component = PartDataToController[IdToPartData[guid]];
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
            foreach (var pair in PartDataToController) {
                pair.Value.SetBlockState(pair.Key.BlockStateType);
            }
            foreach (var pair in PartDataToController) {
                pair.Value.SetLightState(pair.Key.LightStateType);
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
            foreach (var data in IdToPartData) {
                data.Value.SetLightState(data.Value.StateType == StateType.Active ? LightStateType.Enable : LightStateType.Disable);
            }
        }

        #endregion
    }
}
