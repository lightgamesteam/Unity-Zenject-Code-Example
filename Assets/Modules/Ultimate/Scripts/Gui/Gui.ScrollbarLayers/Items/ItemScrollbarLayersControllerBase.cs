using System;
using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiScrollbar;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    [Serializable]
    public abstract class ItemScrollbarLayersControllerBase<TModel, TView> : ItemScrollbarControllerBase<TModel, TView> 
        where TModel : ItemScrollbarLayersModelBase
        where TView : ItemScrollbarLayersViewBase {
        [Inject] private readonly ILayerListeners _managerLayerListeners = default;
        
        public override void Initialize() {
            base.Initialize();
            _managerLayerListeners.OnSwitchModelEvent += ChangeOfLayer;
            _managerLayerListeners.OnSwitchGroupWithModelEvent += ChangeOfLayer;
        }
        
        public override void Initialize(ItemScrollbarModelBase model) {
            base.Initialize(model);
            View.SelectButton.GetComponent<GuiButtonHandlerScrollbarLayersItem>().Initialize(Model.Id);
        }
        
        protected virtual void ChangeOfLayer(int id, bool elementActive) {
            if (!Model.Id.Equals(id)) { return; }
            
            Model.SelectStateType = elementActive ? SelectStateType.Active : SelectStateType.None;
            RefreshView();
        }
        
        protected virtual void ChangeOfLayer(int id, bool layerActive, bool labelActive) { }
        
        protected override void ChangeOfLanguage(ILanguageHandler languageHandler) {
            base.ChangeOfLanguage(languageHandler);
            if (Model.LocalNames == null) { return; }
            
            Model.DisplayLabel = languageHandler.GetCurrentTranslations(Model.LocalNames);
            RefreshView();
        }
    }
}