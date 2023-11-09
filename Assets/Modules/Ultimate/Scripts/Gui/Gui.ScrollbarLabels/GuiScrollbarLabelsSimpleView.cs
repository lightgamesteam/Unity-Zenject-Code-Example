using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public class GuiScrollbarLabelsSimpleView : Gui.Core.ViewBase {
        [SerializeField] public MonoScrollbarLabelsController Content;
    }
}