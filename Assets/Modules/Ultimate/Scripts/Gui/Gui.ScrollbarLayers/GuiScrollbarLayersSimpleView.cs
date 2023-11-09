using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    [Serializable]
    public class GuiScrollbarLayersSimpleView : Gui.Core.ViewBase {
        [SerializeField] public MonoScrollbarLayersController Content;
    }
}