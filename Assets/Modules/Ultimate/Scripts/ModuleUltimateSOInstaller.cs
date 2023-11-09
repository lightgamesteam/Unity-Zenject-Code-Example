using System;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    [CreateAssetMenu(fileName = "ModuleUltimateSOInstaller", menuName = "Installers/ModuleUltimateSOInstaller")]
    public class ModuleUltimateSOInstaller : ScriptableObjectInstaller<ModuleUltimateSOInstaller> {
        [SerializeField] private GuiPrefabs _guiStandalonePrefabs;
        [SerializeField] private GuiPrefabs _guiMobilePrefabs;
        [SerializeField] private GuiItemPrefabs _guiItemStandalonePrefabs;
        [SerializeField] private GuiItemPrefabs _guiItemMobilePrefabs;

        public override void InstallBindings() {
            switch (DeviceInfo.GetUI()) {
                case DeviceUI.PC:
                    Container.BindInstance(_guiStandalonePrefabs);
                    Container.BindInstance(_guiItemStandalonePrefabs);
                    break;
                case DeviceUI.Mobile:
                    Container.BindInstance(_guiMobilePrefabs);
                    Container.BindInstance(_guiItemMobilePrefabs);
                    break;
                default:
                    Container.BindInstance(_guiStandalonePrefabs);
                    Container.BindInstance(_guiItemStandalonePrefabs);
                    break;
            }
            
        }
    }
    
    [Serializable]
    public class GuiPrefabs {
        [SerializeField] public GameObject GuiBackground;
        [SerializeField] public GameObject GuiScrollbarLanguages;
        [SerializeField] public GameObject GuiScrollbarLabels;
        [SerializeField] public GameObject GuiScrollbarLayers;
        [SerializeField] public GameObject GuiControlElements;
        [SerializeField] public GameObject GuiColorPicker;
        [SerializeField] public GameObject GuiTooltip;
    }
    
    [Serializable]
    public class GuiItemPrefabs {
        [Header("Scrollbar - Languages")]
        [SerializeField] public GameObject ItemScrollbarLanguages;
        [Header("Scrollbar - Labels")]
        [SerializeField] public GameObject ItemScrollbarLabelsGroup;
        [SerializeField] public GameObject ItemScrollbarLabelsLayer;
        [SerializeField] public GameObject ItemScrollbarLabelsLabel;
        [Header("Scrollbar - Layers")]
        [SerializeField] public GameObject ItemScrollbarLayersGroup;
        [SerializeField] public GameObject ItemScrollbarLayersLayer;
        [SerializeField] public GameObject ItemScrollbarLayersLabel;
        [Header("Scene.Element - Labels")]
        [SerializeField] public GameObject ItemSceneElementLabel;
        [SerializeField] public GameObject ItemSceneElementLabelCanvas;
    }
}