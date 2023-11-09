using System;
using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.Core.Managers;
using TDL.Modules.Ultimate.GuiScrollbar;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public abstract class ItemScrollbarLabelsControllerBase<TModel, TView> : ItemScrollbarControllerBase<TModel, TView> 
        where TModel : ItemScrollbarLabelsModelBase
        where TView : ItemScrollbarLabelsViewBase {
        [Inject] private readonly ILabelListeners _managerLabelListeners = default;
        
        public override void Initialize() {
            base.Initialize();
            _managerLabelListeners.OnSwitchModelEvent += ChangeOfLabel;
            _managerLabelListeners.OnSwitchGroupWithModelEvent += ChangeOfLabel;
        }
        
        public override void Initialize(ItemScrollbarModelBase model) {
            base.Initialize(model);
            View.SelectButton.GetComponent<GuiButtonHandlerScrollbarLabelsItem>().Initialize(Model.Id);
        }

        protected virtual void ChangeOfLabel(int id, bool elementActive) {
            if (!Model.Id.Equals(id)) { return; }
            
            Model.SelectStateType = elementActive ? SelectStateType.Active : SelectStateType.None;
            RefreshView();
        }

        protected virtual void ChangeOfLabel(int id, bool layerActive, bool labelActive) { }

        protected override void ChangeOfLanguage(ILanguageHandler languageHandler) {
            base.ChangeOfLanguage(languageHandler);
            if (Model.LocalNames == null) { return; }
            
            Model.DisplayLabel = languageHandler.GetCurrentTranslations(Model.LocalNames);
            RefreshView();
        }
    }
}