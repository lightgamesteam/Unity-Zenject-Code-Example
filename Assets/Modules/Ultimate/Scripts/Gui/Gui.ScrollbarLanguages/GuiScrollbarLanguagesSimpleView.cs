using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbarLanguages {
    [Serializable]
    public class GuiScrollbarLanguagesSimpleView : Gui.Core.ViewBase {
        [SerializeField] public MonoScrollbarLanguagesController Content;
    }
}