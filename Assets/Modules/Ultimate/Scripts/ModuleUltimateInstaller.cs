using System;
using Gui.Core.Fsm;
using Module.Common;
using Module.Core.UIComponent;
using TDL.Modules.Model3D;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiBackground;
using TDL.Modules.Ultimate.GuiColorPicker;
using TDL.Modules.Ultimate.GuiControlElements;
using TDL.Modules.Ultimate.GuiScrollbarLabels;
using TDL.Modules.Ultimate.GuiScrollbarLanguages;
using TDL.Modules.Ultimate.GuiScrollbarLayers;
using TDL.Modules.Ultimate.GuiTooltip;
using TDL.Modules.Ultimate.Signal;
using TDL.Modules.Ultimate.States;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class ModuleUltimateInstaller : MonoInstaller {
        [Inject] private readonly GuiPrefabs _guiPrefabs = default;

        private const string PATH_TO_CANVAS_OVERLAY = "Gui/Canvas";
        
        public override void InstallBindings() {
            InstallSignals();
            
            Container.Bind<ManagerActivityData>().AsSingle().NonLazy();
            Container.Bind<ModuleEntryPoint>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<FsmService>().FromInstance(new FsmService(Container)).AsSingle().NonLazy();
            Container.Bind<ComponentObjectVisibility>().FromComponentInHierarchy().AsSingle();
            Container.Bind(typeof(ILanguageHandler), typeof(ILanguageListeners)).To<ManagerLanguage>().AsSingle().NonLazy();
            Container.Bind(typeof(IModelHandler), typeof(IDisposable)).To<ManagerModel>().AsSingle().NonLazy();
            Container.Bind(typeof(ILayerHandler), typeof(ILayerListeners)).To<ManagerLayer>().AsSingle().NonLazy();
            Container.Bind(typeof(ILabelHandler), typeof(ILabelListeners)).To<ManagerLabel>().AsSingle().NonLazy();
            
            InstallGuiBindings();
        }

        public override void Start() {
            base.Start();
            Container.Resolve<FsmService>().RegisterStates<IFsmStateUltimate>();
            switch (DeviceInfo.GetUI()) {
                case DeviceUI.Mobile:
                    Container.Resolve<FsmService>().SetState<FsmStateUltimateMainMobile>();
                    break;
                default:
                    Container.Resolve<FsmService>().SetState<FsmStateUltimateMainStandalone>();
                    break;
            }
        }
        
        private void InstallSignals() {
            Container.DeclareSignal<DownloadBackgroundCommandSignal>();
            Container.BindSignal<DownloadBackgroundCommandSignal>().ToMethod<DownloadBackgroundCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<FullscreenCommandSignal>();
            Container.BindSignal<FullscreenCommandSignal>().ToMethod<FullscreenCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ControlElementsHideAllPanelsCommandSignal>();
            Container.BindSignal<ControlElementsHideAllPanelsCommandSignal>().ToMethod<ControlElementsMultiCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ControlElementsHideAllPanelsExceptCommandSignal>();
            Container.BindSignal<ControlElementsHideAllPanelsExceptCommandSignal>().ToMethod<ControlElementsMultiCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ControlElementsHidePanelCommandSignal>();
            Container.BindSignal<ControlElementsHidePanelCommandSignal>().ToMethod<ControlElementsMultiCommand>(signal => signal.Execute).FromNew();
        }

        private void InstallGuiBindings() {
            switch (DeviceInfo.GetUI()) {
                case DeviceUI.Mobile:
                    GuiBind<GuiBackgroundController>(_guiPrefabs.GuiBackground, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiControlElementsMultiController>(_guiPrefabs.GuiControlElements, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiScrollbarLanguagesSimpleController>(_guiPrefabs.GuiScrollbarLanguages, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiScrollbarLabelsSimpleController>(_guiPrefabs.GuiScrollbarLabels, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiScrollbarLayersSimpleController>(_guiPrefabs.GuiScrollbarLayers, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiColorPickerSimpleController>(_guiPrefabs.GuiColorPicker, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiTooltipController>(_guiPrefabs.GuiTooltip, PATH_TO_CANVAS_OVERLAY);
                    break;
                default:
                    GuiBind<GuiBackgroundController>(_guiPrefabs.GuiBackground, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiScrollbarLanguagesSimpleController>(_guiPrefabs.GuiScrollbarLanguages, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiScrollbarLabelsSimpleController>(_guiPrefabs.GuiScrollbarLabels, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiScrollbarLayersSimpleController>(_guiPrefabs.GuiScrollbarLayers, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiControlElementsSimpleController>(_guiPrefabs.GuiControlElements, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiColorPickerSimpleController>(_guiPrefabs.GuiColorPicker, PATH_TO_CANVAS_OVERLAY);
                    GuiBind<GuiTooltipController>(_guiPrefabs.GuiTooltip, PATH_TO_CANVAS_OVERLAY);
                    break;
            }
        }
        
        private void GuiBind<T>(UnityEngine.Object prefab, string parentGroup) where T : Gui.Core.ControllerBase {
            Container.Bind(typeof(T), typeof(IDisposable)).To<T>().FromComponentInNewPrefab(prefab).UnderTransformGroup(parentGroup).AsSingle().NonLazy();
        }
    }
}