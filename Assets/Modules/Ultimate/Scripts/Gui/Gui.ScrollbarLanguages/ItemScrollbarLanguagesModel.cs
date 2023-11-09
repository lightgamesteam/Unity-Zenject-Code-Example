using System;
using Module.Core.Attributes;
using TDL.Modules.Ultimate.GuiScrollbar;
using TDL.Server;

namespace TDL.Modules.Ultimate.GuiScrollbarLanguages {
    [Serializable]
    public class ItemScrollbarLanguagesModel : ItemScrollbarModelBase {
        [ShowOnly] public LanguageResource Data;

        public override void SetModel(ItemScrollbarModelBase model) {
            base.SetModel(model);
            if (model is ItemScrollbarLanguagesModel newModel) {
                Data = newModel.Data;
            }
        }
    }
}