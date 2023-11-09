using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Module.Bundle;
using Module.Core.Data;
using UnityEngine;

namespace Module.Core.Model {
    public abstract class ModelControllerBase {
        #region Variables

        public readonly ModelDataBase Data;
        public readonly ICurrentObjectHandler CurrentObjectHandler;

        #endregion

        #region Public methods

        protected ModelControllerBase(Transform parent, ModelDataBase data, AssetBundle assetBundle) {
            Data = data;
            CurrentObjectHandler = assetBundle != null 
                ? CurrentObjectController.ConnectCurrentObjectHandler(assetBundle) 
                : CurrentObjectController.ConnectCurrentObjectHandler(data.FileHash);
            CurrentObjectHandler.Instantiate(parent, data.ModelName, data.PurposeName);
        }

        public void SetVisible(bool isActive) {
            CurrentObjectHandler?.SetActive(isActive);
        }

        public void Dispose() {
            CurrentObjectHandler.Dispose();
        }

        #endregion

        #region Protected methods

        protected static Dictionary<Transform, string[]> GetModifiedPartLabelsFromGameObject(IEnumerable<GameObject> source) {
            var result = new Dictionary<Transform, string[]>();
            foreach (var gameObject in source) {
                var transform = gameObject.GetComponent<Transform>();
                for (var i = 0; i < transform.childCount; i++) {
                    var partTransform = transform.GetChild(i);
                    var partName = partTransform.name;
                    var words = GetModifiedLabels(partName);
                    result.Add(partTransform, words);
                }
            }
            return result;
        }

        protected static Dictionary<T, List<string[]>> GetModifiedPartLabelsFromData<T>(IEnumerable<T> source) where T : ModelPartData {
            var result = new Dictionary<T, List<string[]>>();
            foreach (var modelPartData in source) {
                var valueList = new List<string[]>();
                foreach (var name in modelPartData.LabelName) {
                    if (!string.IsNullOrEmpty(name)) {
                        var words = GetModifiedLabels(name);
                        valueList.Add(words);
                    }
                }
                result.Add(modelPartData, valueList);
            }
            return result;
        }

        protected static string[] GetModifiedLabels(string value) {
            const string pattern = @"([A-Z0-9]+[a-z0-9]+)|([A-Z0-9]+)|([a-z0-9]+)";
            var words = Regex.Matches(value, pattern).OfType<Match>().Select(m => m.Value);
            return words.Select(word => word.ToLower()).ToArray();
        }

        #endregion
    }
}