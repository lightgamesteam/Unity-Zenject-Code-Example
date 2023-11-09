using System.Collections.Generic;
using System.Linq;
using Gui.Core;
using TDL.Constants;
using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.Core.Elements;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiScrollbar;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    public class MonoScrollbarLayersController : MonoViewControllerBase<MonoScrollbarLayersView> {
        [Inject] private readonly ILanguageHandler _managerLanguageHandler = default;
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        [Inject] private readonly ILayerHandler _managerLayerHandler = default;
        [Inject] private readonly GuiItemPrefabs _guiItemPrefabs = default;
        
        private readonly List<ItemScrollbarLayersControllerGroup> _groupControllerArray = new List<ItemScrollbarLayersControllerGroup>();
        
        public override void Initialize() {
            base.Initialize();
            ClearContent();
            CreateContent();
            SortContent();

            _managerLanguageListeners.LocalizeEvent += Localization;
            _managerLanguageListeners.Invoke(Localization);
            _managerLanguageListeners.LastLocalizeEvent += handler => { SortContent(); };
        }
        
        #region Private methods

        private void ClearContent() {
            if (View.ScrollContent.childCount != 0) {
                for (var i = View.ScrollContent.childCount - 1; i >= 0; i--) {
                    Destroy(View.ScrollContent.GetChild(i).gameObject);
                }
            }
            _groupControllerArray.Clear();
        }

        private void CreateContent() {
            foreach (var groupController in _managerLayerHandler.GetGroupControllerArray()) {
                var layerModel = CreateModel<ItemScrollbarLayersModelLayer>(groupController.LayerModelController);
                var labelModels = new ItemScrollbarLayersModelLabel[groupController.LabelModelControllers.Length];
                for (var index = 0; index < labelModels.Length; index++) {
                    labelModels[index] = CreateModel<ItemScrollbarLayersModelLabel>(groupController.LabelModelControllers[index]);
                }
                var component = InstantiatePrefab<ItemScrollbarLayersControllerGroup>(_guiItemPrefabs.ItemScrollbarLayersGroup, View.ScrollContent);
                component.Initialize(layerModel, labelModels);
                _groupControllerArray.Add(component);
            }
        }
        
        private T CreateModel<T>(ModelController controller) where T : ItemScrollbarLayersModelBase, new() {
            return new T {
                Id = controller.Data.Id, 
                LocalNames = controller.Data.LocalNames,
                DisplayLabel = _managerLanguageHandler.GetCurrentTranslations(controller.Data.LocalNames), 
                ItemStateType = ItemStateType.Enable, 
                SelectStateType = controller.IsActive ? SelectStateType.Active : SelectStateType.None
            };
        }
        
        private void SortContent() {
            var sortedArray = _groupControllerArray.OrderBy(x => x.DisplayLabelForOrder).ToArray();
            for (var index = 0; index < sortedArray.Length; index++) {
                sortedArray[index].transform.SetSiblingIndex(index);
            }
            
            var selectAllController = _managerLayerHandler.GetSelectAllController();
            foreach (var controllerGroup in sortedArray) {
                if (controllerGroup.Id.Equals(selectAllController.LayerModelController.Data.Id)) {
                    controllerGroup.transform.SetSiblingIndex(0);
                    break;
                }
            }
        }
        
        private void Localization(ILanguageHandler languageHandler) {
            if (View.TitleText != null) {
                View.TitleText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.LayersKey);
            }
            if (View.CloseText != null) {
                View.CloseText.text = languageHandler.GetCurrentTranslations(LocalizationConstants.CloseKey);
            }
        }

        #endregion
    }
}
