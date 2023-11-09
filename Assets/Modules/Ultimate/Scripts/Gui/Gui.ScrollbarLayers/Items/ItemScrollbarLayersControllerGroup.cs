using System.Linq;
using Gui.Core;
using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    public class ItemScrollbarLayersControllerGroup : MonoViewControllerBase<ItemScrollbarLayersViewGroup> {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        [Inject] private readonly GuiItemPrefabs _guiItemPrefabs = default;
        
        public int Id => _layerController.Model.Id;
        public string DisplayLabelForOrder => _layerController.Model.DisplayLabel;
        
        private ItemScrollbarLayersControllerLayer _layerController;
        private ItemScrollbarLayersControllerLabel[] _labelControllerArray;
        private SubContentType _subContentType;
        
        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LastLocalizeEvent += handler => { SortContent(); };
        }
        
        public void Initialize(ItemScrollbarLayersModelLayer modelLayer, ItemScrollbarLayersModelLabel[] modelLabels) {
            CreateContent(modelLayer, modelLabels);
            SetSubContent(_subContentType);
            SortContent();
        }

        private void CreateContent(ItemScrollbarLayersModelLayer modelLayer, ItemScrollbarLayersModelLabel[] modelLabels) {
            _layerController = InstantiatePrefab<ItemScrollbarLayersControllerLayer>(_guiItemPrefabs.ItemScrollbarLayersLayer);
            _layerController.Initialize(modelLayer);
            
            _labelControllerArray = new ItemScrollbarLayersControllerLabel[modelLabels.Length];
            for (var i = 0; i < _labelControllerArray.Length; i++) {
                var labelController = InstantiatePrefab<ItemScrollbarLayersControllerLabel>(_guiItemPrefabs.ItemScrollbarLayersLabel);
                labelController.Initialize(modelLabels[i]);
                _labelControllerArray[i] = labelController;
            }

            _subContentType = _labelControllerArray.Length == 0 ? SubContentType.None : SubContentType.Hide;
            _layerController.InitializeSubContent(_subContentType, SwitchContent);
        }

        private void SwitchContent() {
            switch (_subContentType) {
                case SubContentType.Show:
                    SetSubContent(SubContentType.Hide);
                    break;
                case SubContentType.Hide:
                    SetSubContent(SubContentType.Show);
                    break;
            }
        }
        
        private void SetSubContent(SubContentType contentType) {
            _subContentType = contentType;
            _layerController.SetSubContent(contentType);
            switch (contentType) {
                case SubContentType.Show:
                    if (_labelControllerArray != null) {
                        foreach (var part in _labelControllerArray) {
                            part.gameObject.SetActive(true);
                        }
                    }
                    break;
                case SubContentType.None:
                case SubContentType.Hide:
                default:
                    if (_labelControllerArray != null) {
                        foreach (var part in _labelControllerArray) {
                            part.gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }
        
        private void SortContent() {
            var sortedArray = _labelControllerArray.OrderBy(x => x.Model.DisplayLabel).ToArray();
            for (var index = 0; index < sortedArray.Length; index++) {
                sortedArray[index].transform.SetSiblingIndex(index + 1); //Sibling index 0 - layer controller
            }
        }
    }
}