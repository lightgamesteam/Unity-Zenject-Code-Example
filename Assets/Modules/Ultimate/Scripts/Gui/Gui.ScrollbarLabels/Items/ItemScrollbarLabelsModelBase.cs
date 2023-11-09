using System;
using Module.Core.Attributes;
using TDL.Modules.Ultimate.GuiScrollbar;
using TDL.Server;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public abstract class ItemScrollbarLabelsModelBase : ItemScrollbarModelBase {
        [ShowOnly] public int Id;
        [ShowOnly] public LocalName[] LocalNames;

        public override void SetModel(ItemScrollbarModelBase model) {
            base.SetModel(model);
            if (model is ItemScrollbarLabelsModelBase newModel) {
                Id = newModel.Id;
                LocalNames = newModel.LocalNames;
            }
        }
    }
}