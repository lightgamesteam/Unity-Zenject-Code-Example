using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Module.Bundle;
using Module.Core;
using Module.Core.UIComponent;
using TDL.Modules.Ultimate.Core.ActivityData;
using TDL.Modules.Ultimate.Core.Elements;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Ultimate.Core.Managers {
    public class ManagerModel : ManagerBase, IDisposable, IModelHandler {
        [Inject] private readonly ComponentObjectVisibility _visibility = default;
        [Inject] private readonly ManagerActivityData _activityData = default;
        
        private ICurrentObjectHandler _currentObjectHandler;
        private Dictionary<int, ModelController> _modelById;
        
        protected override void Construct(DiContainer container) {
            base.Construct(container);
            _currentObjectHandler = CurrentObjectController.ConnectCurrentObjectHandler(_activityData.FileHash);
            _currentObjectHandler.Instantiate(_visibility.transform, _activityData.ActivityLocal.First().Name, "Mesh");
            _modelById = CreatePartById(_currentObjectHandler.Instances, _activityData.GroupDataArray);
        }

        #region IDisposable
        
        public void Dispose() {
            _currentObjectHandler.Dispose();
        }
        
        #endregion

        #region IModelHandler
        
        public void SetHighlightPart(int id, bool isActive) {
            if (_modelById.ContainsKey(id)) {
                _modelById[id].SetHighlightColor(isActive);
            }
        }
        
        public ModelController GetController(int id) {
            return TryGetController(id, out var controller) ? controller : null;
        }
        
        public bool TryGetController(int id, out ModelController controller) {
            return _modelById.TryGetValue(id, out controller);
        }
        
        #endregion
        
        #region Private methods

        private static Dictionary<int, ModelController> CreatePartById(IReadOnlyCollection<GameObject> instances, IReadOnlyCollection<GroupData> groups) {
            var result = new Dictionary<int, ModelController>();
            if (instances.Count == 0) { return result; }
            
            var instanceDictionaryWithListOfWords = Utilities.Transform.GetModifiedPartLabelsFromGameObject(instances);
            var activityDictionaryWithListOfWords = GetDictionaryWithListOfWords(groups);
            
            var debugAssert = new StringBuilder();
            foreach (var activityPairWithListOfWords in activityDictionaryWithListOfWords) {
                if (IsEqualCompare(activityPairWithListOfWords, instanceDictionaryWithListOfWords, out var controller)) {
                    if (!result.ContainsKey(activityPairWithListOfWords.Key.Id)) {
                        result.Add(activityPairWithListOfWords.Key.Id, controller);
                    } else {
                        debugAssert.AppendLine(GetAssertMessage(activityPairWithListOfWords.Key, "Duplicated name in different places"));
                        // Debug.LogWarning(GetAssertMessage(activityPairWithListOfWords.Key, "Duplicated name in different places"));
                    }
                } else {
                    debugAssert.AppendLine(GetAssertMessage(activityPairWithListOfWords.Key, "Not found at GameObject"));
                    // Debug.LogWarning(GetAssertMessage(activityPairWithListOfWords.Key, "Not found at GameObject"));
                }
            }
            if (debugAssert.Length != 0) {
                Debug.LogWarning(debugAssert);
            }
            return result;
        }
        
        private static bool IsEqualCompare(KeyValuePair<DataBase, List<string[]>> activityPair, Dictionary<Transform, string[]> instanceDictionary, out ModelController controller) {
            foreach (var instancePair in instanceDictionary) {
                foreach (var activityPairValue in activityPair.Value) {
                    if (instancePair.Value.SequenceEqual(activityPairValue)) {
                        controller = new ModelController(instancePair.Key, activityPair.Key);
                        return true;
                    }
                }
            }
            controller = default;
            return false;
        }

        private static Dictionary<DataBase, List<string[]>> GetDictionaryWithListOfWords(IEnumerable<GroupData> source) {
            var result = new Dictionary<DataBase, List<string[]>>();
            foreach (var groupDataBase in source) {
                result.Add(groupDataBase.LayerData, GetListOfWords(groupDataBase.LayerData));
                foreach (var labelDataBase in groupDataBase.LabelDataArray) {
                    result.Add(labelDataBase, GetListOfWords(labelDataBase));
                }
            }
            return result;
        }
        
        private static List<string[]> GetListOfWords(DataBase dataBase) {
            var result = new List<string[]>();
            if (string.IsNullOrEmpty(dataBase.GoName)) {
                result.AddRange(
                    from localName in dataBase.LocalNames 
                    where !string.IsNullOrEmpty(localName.Name) 
                    select Utilities.Transform.GetModifiedLabels(localName.Name));
            } else {
                result.Add(Utilities.Transform.GetModifiedLabels(dataBase.GoName));
            }
            return result;
        }
        
        private static string GetAssertMessage(DataBase dataBase, string message) {
            var findingName = string.IsNullOrEmpty(dataBase.GoName) ? dataBase.LocalNames.First().Name : dataBase.GoName;
            return $"[ASSERT] {message} -> Id:{dataBase.Id} Name:{findingName}";
        }
        
        #endregion
    }
    
    public interface IModelHandler {
        void SetHighlightPart(int id, bool isActive);
        ModelController GetController(int id);
        bool TryGetController(int id, out ModelController controller);
    }
}