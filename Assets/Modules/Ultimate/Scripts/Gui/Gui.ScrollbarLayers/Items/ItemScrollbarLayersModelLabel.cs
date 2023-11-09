using System;
using Module.Core.Attributes;
using TDL.Modules.Ultimate.GuiScrollbar;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    [Serializable]
    public class ItemScrollbarLayersModelLabel : ItemScrollbarLayersModelBase {
        [ShowOnly] public EyeStateType EyeStateType;

        public override void SetModel(ItemScrollbarModelBase model) {
            base.SetModel(model);
            if (model is ItemScrollbarLayersModelLabel newModel) {
                EyeStateType = newModel.EyeStateType;
            }
        }
    }
}