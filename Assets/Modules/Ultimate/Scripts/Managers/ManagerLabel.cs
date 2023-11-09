using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Module.Core.UIComponent;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.ActivityData;
using TDL.Modules.Ultimate.Core.Elements;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace TDL.Modules.Ultimate.Core.Managers {
    public class ManagerLabel : ManagerBase, ILabelListeners, ILabelHandler {
        [Inject] private readonly ComponentObjectVisibility _visibility = default;
        [Inject] private readonly ManagerActivityData _activityData = default;
        [Inject] private readonly ILayerListeners _managerLayerListeners = default;
        [Inject] private readonly IModelHandler _managerModelHandler = default;
        [Inject] private readonly ILanguageHandler _managerLanguageHandler = default;
        [Inject] private readonly GuiItemPrefabs _itemPrefabs = default;
        
        private GameObject _componentVisibility;
        private LabelGroupController _selectAllController;
        private readonly HashSet<LabelGroupController> _groupControllers = new HashSet<LabelGroupController>();
        private readonly Dictionary<int, LabelGroupController> _groupControllerByModelId = new Dictionary<int, LabelGroupController>();
        private readonly Dictionary<int, LabelController> _layerControllerById = new Dictionary<int, LabelController>();
        private readonly Dictionary<int, LabelController> _labelControllerById = new Dictionary<int, LabelController>();
        
        protected override void Construct(DiContainer container) {
            base.Construct(container);
            _componentVisibility = new GameObject($"Model: {_activityData.ActivityLocal.First().Name} - Labels");
            _componentVisibility.transform.SetParent(_visibility.transform);
            _componentVisibility.transform.localPosition = Vector3.zero;
            _componentVisibility.transform.localScale = Vector3.one;
            CreateDictionaries(_activityData.GroupDataArray);
            SetActiveAll(false);
            
            _managerLayerListeners.OnSwitchModelEvent += SetModel;
        }

        #region ILabelListeners

        public event Action<int, bool, bool> OnSwitchGroupWithModelEvent;
        public event Action<int, bool> OnSwitchModelEvent;
        
        #endregion
        
        #region ILabelHandler
        
        public LabelGroupController[] GetGroupControllerArray() {
            return _groupControllers.ToArray();
        }
        
        public LabelGroupController GetSelectAllController() {
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
                    layerController.SetActiveAndRefresh(false);
                    OnSwitchModelEvent?.Invoke(layerController.Data.Id, layerController.IsActive);
                    DeactivateSelectAll();
                }
            }
            {// Set HighlightPart
                if (_layerControllerById.ContainsKey(id)) {
                    if (_selectAllController.LayerModelController.Data.Id.Equals(id)) {
                        foreach (var controller in _groupControllers) {
                            SendSignalToSetHighlightPartFromLabels(controller.LabelModelControllers);
                        }
                    } else {
                        SendSignalToSetHighlightPartFromLabels(_groupControllerByModelId[id].LabelModelControllers);
                    }
                } else if (_labelControllerById.ContainsKey(id)) {
                    var labelController = _labelControllerById[id];
                    _managerModelHandler.SetHighlightPart(labelController.Data.Id, labelController.IsActive);
                }
            }
        }
        
        public void SetModel(int id, bool elementActive) {
            {// Set model in LayerController
                if (_layerControllerById.ContainsKey(id)) {
                    var layerController = _layerControllerById[id];
                    OnSwitchGroupWithModelEvent?.Invoke(layerController.Data.Id, elementActive, layerController.IsActive);
                }
            }
            {// Set model in LabelController
                if (_labelControllerById.ContainsKey(id)) {
                    var labelController = _labelControllerById[id];
                    OnSwitchGroupWithModelEvent?.Invoke(labelController.Data.Id, elementActive, labelController.IsActive);
                }
            }
        }

        #endregion
        
        #region Private methods
        
        private void CreateDictionaries(IEnumerable<GroupData> groupDataArray) {
            foreach (var groupDataBase in groupDataArray) {
                var layerView = InstantiatePrefab(_componentVisibility.transform, groupDataBase.LayerData);
                var layerController = new LabelController(layerView, groupDataBase.LayerData);
                var labelControllerList = new List<LabelController>();
                foreach (var labelDataBase in groupDataBase.LabelDataArray) {
                    if (_managerModelHandler.TryGetController(labelDataBase.Id, out _)) {
                        var labelView = InstantiatePrefab(_componentVisibility.transform, labelDataBase);
                        labelControllerList.Add(new LabelController(labelView, labelDataBase));
                    }
                }
                var groupController = new LabelGroupController(groupDataBase, layerController, labelControllerList.ToArray());
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
                var layerController = new LabelController(layerView, groupDataBase.LayerData);
                var groupController = new LabelGroupController(groupDataBase, layerController, new LabelController[0]);
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
        
        private Transform InstantiatePrefab(Transform parent, ActivityData.LabelData data) {
            var prefab = _itemPrefabs.ItemSceneElementLabel;
            Assert.IsNotNull(prefab);
            var go = Container.InstantiatePrefab(prefab);
            go.name = $"Label: {data.LocalNames.First().Name} {data.Id}";
            go.transform.SetParent(parent);
            go.transform.localPosition = data.Position;
            go.transform.localRotation = data.Rotation;
            go.transform.localScale = Vector3.one;
            return go.transform;
        }
        
        private void AddModelIdToGroupControllerDictionary(LabelGroupController groupController, params LabelController[] labelControllers) {
            if (labelControllers == null) { return; }
            
            var debugAssert = new StringBuilder();
            foreach (var labelController in labelControllers) {
                if (!_groupControllerByModelId.ContainsKey(labelController.Data.Id)) {
                    _groupControllerByModelId.Add(labelController.Data.Id, groupController);
                    _groupControllers.Add(groupController);
                } else {
                    debugAssert.AppendLine(GetAssertMessage(labelController.Data, "Contains a key in the group dictionary"));
                }
            }
            if (debugAssert.Length != 0) {
                Debug.LogWarning(debugAssert);
            }
        }
        
        private void AddModelIdToLayerControllerDictionary(params LabelController[] labelControllers) {
            if (labelControllers == null) { return; }

            var debugAssert = new StringBuilder();
            foreach (var labelController in labelControllers) {
                if (!_layerControllerById.ContainsKey(labelController.Data.Id)) {
                    _layerControllerById.Add(labelController.Data.Id, labelController);
                } else {
                    debugAssert.AppendLine(GetAssertMessage(labelController.Data, "Contains a key in the layer dictionary"));
                }
            }
            if (debugAssert.Length != 0) {
                Debug.LogWarning(debugAssert);
            }
        }
        
        private void AddModelIdToLabelControllerDictionary(params LabelController[] labelControllers) {
            if (labelControllers == null) { return; }

            var debugAssert = new StringBuilder();
            foreach (var labelController in labelControllers) {
                if (!_labelControllerById.ContainsKey(labelController.Data.Id)) {
                    _labelControllerById.Add(labelController.Data.Id, labelController);
                } else {
                    debugAssert.AppendLine(GetAssertMessage(labelController.Data, "Contains a key in the label dictionary"));
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
        
        private void SendSignalToSetHighlightPartFromLabels(IEnumerable<LabelController> controllers) {
            foreach (var labelController in controllers) {
                _managerModelHandler.SetHighlightPart(labelController.Data.Id, labelController.IsActive);
            }
        }
        
        private void DeactivateSelectAll() {
            var layerController = _selectAllController.LayerModelController;
            layerController.SetActiveAndRefresh(false);
            OnSwitchModelEvent?.Invoke(layerController.Data.Id, layerController.IsActive);
        }

        #endregion
    }
    
    public interface ILabelListeners {
        event Action<int, bool, bool> OnSwitchGroupWithModelEvent;
        event Action<int, bool> OnSwitchModelEvent;
    }
    
    public interface ILabelHandler {
        LabelGroupController[] GetGroupControllerArray();
        LabelGroupController GetSelectAllController();
        void SetModel(int id);
        void SetModel(int id, bool elementActive);
    }
}