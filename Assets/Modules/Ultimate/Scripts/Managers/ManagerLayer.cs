using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Module.Core.UIComponent;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.ActivityData;
using TDL.Modules.Ultimate.Core.Elements;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Ultimate.Core.Managers {
    public class ManagerLayer : ManagerBase, ILayerListeners, ILayerHandler {
        [Inject] private readonly ComponentObjectVisibility _visibility = default;
        [Inject] private readonly ManagerActivityData _activityData = default;
        [Inject] private readonly IModelHandler _managerModelHandler = default;
        [Inject] private readonly ILanguageHandler _managerLanguageHandler = default;

        private GameObject _componentVisibility;
        private LayerGroupController _selectAllController;
        private readonly HashSet<LayerGroupController> _groupControllers = new HashSet<LayerGroupController>();
        private readonly Dictionary<int, LayerGroupController> _groupControllerByModelId = new Dictionary<int, LayerGroupController>();
        private readonly Dictionary<int, ModelController> _layerControllerById = new Dictionary<int, ModelController>();
        private readonly Dictionary<int, ModelController> _labelControllerById = new Dictionary<int, ModelController>();

        protected override void Construct(DiContainer container) {
            base.Construct(container);
            _componentVisibility = new GameObject($"Model: {_activityData.ActivityLocal.First().Name} - Layers");
            _componentVisibility.transform.SetParent(_visibility.transform);
            _componentVisibility.transform.localPosition = Vector3.zero;
            _componentVisibility.transform.localScale = Vector3.one;
            CreateDictionaries(_activityData.GroupDataArray);
            SetActiveAll(true);
        }
        
        #region ILayerListeners

        public event Action<int, bool, bool> OnSwitchGroupWithModelEvent;
        public event Action<int, bool> OnSwitchModelEvent;
        
        #endregion

        #region ILayerHandler
        
        public LayerGroupController[] GetGroupControllerArray() {
            return _groupControllers.ToArray();
        }
        
        public LayerGroupController GetSelectAllController() {
            return _selectAllController;
        }

        public void SetModel(int id) {
            {// Set model in LayerController
                if (_layerControllerById.ContainsKey(id)) {
                    if (_selectAllController.LayerModelController.Data.Id.Equals(id)) {
                        //Select All click
                        SetActiveAll(!_layerControllerById[id].IsActive);
                    } else {
                        var layerController = _layerControllerById[id];
                        layerController.SwitchActiveAndRefresh();
                        OnSwitchModelEvent?.Invoke(layerController.Data.Id, layerController.IsActive);
                        foreach (var labelController in _groupControllerByModelId[id].LabelModelControllers) {
                            labelController.SetActiveAndRefresh(layerController.IsActive);
                            OnSwitchModelEvent?.Invoke(labelController.Data.Id, labelController.IsActive);
                            //OnSwitchGroupWithModelEvent?.Invoke(labelController.Data.Id, layerController.IsActive, labelController.IsActive);
                        }
                        DeactivateSelectAll();
                    }
                }
            }
            {// Set model in LabelController
                if (_labelControllerById.ContainsKey(id)) {
                    var labelController = _labelControllerById[id];
                    labelController.SwitchActiveAndRefresh();
                    OnSwitchModelEvent?.Invoke(labelController.Data.Id, labelController.IsActive);
                    var layerController = _groupControllerByModelId[id].LayerModelController;
                    if (!layerController.IsActive) {
                        layerController.SwitchActiveAndRefresh();
                    }
                    OnSwitchModelEvent?.Invoke(layerController.Data.Id, layerController.IsActive);
                    DeactivateSelectAll();
                }
            }
        }
        
        public void SetModel(int id, bool elementActive) {
        }
        
        #endregion
        
        #region Private methods
        
        private void CreateDictionaries(IEnumerable<GroupData> groupDataArray) {
            foreach (var groupDataBase in groupDataArray) {
                var layerController = _managerModelHandler.GetController(groupDataBase.LayerData.Id);
                var labelControllerList = new List<ModelController>();
                foreach (var labelDataBase in groupDataBase.LabelDataArray) {
                    if (_managerModelHandler.TryGetController(labelDataBase.Id, out var controller)) {
                        labelControllerList.Add(controller);
                    }
                }
                var groupController = new LayerGroupController(groupDataBase, layerController, labelControllerList.ToArray());
                AddModelIdToGroupControllerDictionary(groupController, layerController);
                AddModelIdToGroupControllerDictionary(groupController, labelControllerList.ToArray());
                AddModelIdToLayerControllerDictionary(layerController);
                AddModelIdToLabelControllerDictionary(labelControllerList.ToArray());
            }
            
            {//Select All
                var layerDataBase = new LayerData {
                    Id = 0,
                    LocalNames = _managerLanguageHandler.GetAllTranslations(LocalizationConstants.SelectAllKey)
                };
                var groupDataBase = new GroupData {
                    LayerData = layerDataBase, 
                    LabelDataArray = new ActivityData.LabelData[0]
                };
                var layerView = InstantiatePrefab(_componentVisibility.transform, groupDataBase.LayerData);
                var layerController = new ModelController(layerView, groupDataBase.LayerData);
                var groupController = new LayerGroupController(groupDataBase, layerController, new ModelController[0]);
                AddModelIdToGroupControllerDictionary(groupController, layerController);
                AddModelIdToLayerControllerDictionary(layerController);
                _selectAllController = groupController;
            }
        }
        
        private Transform InstantiatePrefab(Transform parent, DataBase data) {
            var go = new GameObject();
            go.name = $"Void: {data.LocalNames.First().Name} {data.Id}";
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go.transform;
        }
        
        private void AddModelIdToGroupControllerDictionary(LayerGroupController groupController, params ModelController[] modelControllers) {
            if (modelControllers == null) { return; }
            
            var debugAssert = new StringBuilder();
            foreach (var modelController in modelControllers) {
                if (!_groupControllerByModelId.ContainsKey(modelController.Data.Id)) {
                    _groupControllerByModelId.Add(modelController.Data.Id, groupController);
                    _groupControllers.Add(groupController);
                } else {
                    debugAssert.AppendLine(GetAssertMessage(modelController.Data, "Contains a key in the group dictionary"));
                }
            }
            if (debugAssert.Length != 0) {
                Debug.LogWarning(debugAssert);
            }
        }
        
        private void AddModelIdToLayerControllerDictionary(params ModelController[] modelControllers) {
            if (modelControllers == null) { return; }

            var debugAssert = new StringBuilder();
            foreach (var modelController in modelControllers) {
                if (!_layerControllerById.ContainsKey(modelController.Data.Id)) {
                    _layerControllerById.Add(modelController.Data.Id, modelController);
                } else {
                    debugAssert.AppendLine(GetAssertMessage(modelController.Data, "Contains a key in the layer dictionary"));
                }
            }
            if (debugAssert.Length != 0) {
                Debug.LogWarning(debugAssert);
            }
        }
        
        private void AddModelIdToLabelControllerDictionary(params ModelController[] modelControllers) {
            if (modelControllers == null) { return; }

            var debugAssert = new StringBuilder();
            foreach (var modelController in modelControllers) {
                if (!_labelControllerById.ContainsKey(modelController.Data.Id)) {
                    _labelControllerById.Add(modelController.Data.Id, modelController);
                } else {
                    debugAssert.AppendLine(GetAssertMessage(modelController.Data, "Contains a key in the label dictionary"));
                }
            }
            if (debugAssert.Length != 0) {
                Debug.LogWarning(debugAssert);
            }
        }
        
        private static string GetAssertMessage(DataBase dataBase, string message) {
            var findingName = string.IsNullOrEmpty(dataBase.GoName) ? dataBase.LocalNames.First().Name : dataBase.GoName;
            return $"[ASSERT] {message} -> Id:{dataBase.Id} Name:{findingName}";
        }
        
        private void SetActiveAll(bool isActive) {
            foreach (var groupController in _groupControllers) {
                var layerController = groupController.LayerModelController;
                layerController.SetActiveAndRefresh(isActive);
                OnSwitchModelEvent?.Invoke(layerController.Data.Id, layerController.IsActive);
                foreach (var labelController in groupController.LabelModelControllers) {
                    labelController.SetActiveAndRefresh(isActive);
                    OnSwitchModelEvent?.Invoke(labelController.Data.Id, labelController.IsActive);
                }
            }
        }
        
        private void DeactivateSelectAll() {
            var layerController = _selectAllController.LayerModelController;
            layerController.SetActiveAndRefresh(false);
            OnSwitchModelEvent?.Invoke(layerController.Data.Id, layerController.IsActive);
        }
        
        #endregion
    }
    
    public interface ILayerListeners {
        event Action<int, bool, bool> OnSwitchGroupWithModelEvent;
        event Action<int, bool> OnSwitchModelEvent;
    }
    
    public interface ILayerHandler {
        LayerGroupController[] GetGroupControllerArray();
        LayerGroupController GetSelectAllController();
        void SetModel(int id);
        void SetModel(int id, bool elementActive);
    }
}