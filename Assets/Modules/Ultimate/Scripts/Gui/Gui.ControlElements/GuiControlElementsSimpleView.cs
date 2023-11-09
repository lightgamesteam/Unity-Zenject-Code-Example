using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class GuiControlElementsSimpleView : Gui.Core.ViewBase {
        [SerializeField] public MonoControlElementsController Content;
    }
}