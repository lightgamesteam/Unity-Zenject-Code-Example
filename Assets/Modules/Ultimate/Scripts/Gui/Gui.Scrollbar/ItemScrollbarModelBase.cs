using System;
using Module.Core.Attributes;

namespace TDL.Modules.Ultimate.GuiScrollbar {
    [Serializable]
    public abstract class ItemScrollbarModelBase : Gui.Core.ModelBase {
        [ShowOnly] public string DisplayLabel;
        [ShowOnly] public ItemStateType ItemStateType;
        [ShowOnly] public SelectStateType SelectStateType;

        public virtual void SetModel(ItemScrollbarModelBase model) {
            DisplayLabel = model.DisplayLabel;                
            ItemStateType = model.ItemStateType;
            SelectStateType = model.SelectStateType;
        }
    }
}