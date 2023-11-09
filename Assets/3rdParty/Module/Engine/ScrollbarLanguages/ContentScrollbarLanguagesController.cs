using System;
using System.Collections.Generic;
using Module.Core;
using Module.Core.Content;
using Module.Engine.ScrollbarLanguages.Item;

namespace Module.Engine.ScrollbarLanguages.Content {
    public class ContentScrollbarLanguagesController : ContentControllerWithViewBase<ContentScrollbarLanguagesView> {
        protected Dictionary<int, ItemScrollbarLanguagesData> IdToData;
        protected Dictionary<ItemScrollbarLanguagesData, Item.ItemScrollbarLanguagesView> DataToView;

        #region Public methods

        public void Initialize(string prefabPath, IEnumerable<ItemScrollbarLanguagesData> dataArray, Action<int> selectAction) {
            Dispose();

            var prefab = Utilities.Resources.GetGameObject(prefabPath);
            Connect.Dictionary.DataByGuid(dataArray, out IdToData);
            Connect.Dictionary.ControllerByData(dataArray, prefab, View.ScrollContent, out DataToView);
            Connect.Initialize.Component(DataToView, selectAction);
            RefreshState();
        }

        public void SetState(int id, ItemStateType itemStateType) {
            if (IdToData.ContainsKey(id)) {
                IdToData[id].SetState(itemStateType);
            }
            RefreshState();
        }

        public void RefreshState() {
            foreach (var pair in DataToView) {
                pair.Value.SetLightState(pair.Key.SelectStateType);
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
    }
}
