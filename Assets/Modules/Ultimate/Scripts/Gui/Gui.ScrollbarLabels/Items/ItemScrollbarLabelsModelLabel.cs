using System;
using Module.Core.Attributes;
using TDL.Modules.Ultimate.GuiScrollbar;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public class ItemScrollbarLabelsModelLabel : ItemScrollbarLabelsModelBase {
        [ShowOnly] public EyeStateType EyeStateType;

        public override void SetModel(ItemScrollbarModelBase model) {
            base.SetModel(model);
            if (model is ItemScrollbarLabelsModelLabel newModel) {
                EyeStateType = newModel.EyeStateType;
            }
        }
    }
}