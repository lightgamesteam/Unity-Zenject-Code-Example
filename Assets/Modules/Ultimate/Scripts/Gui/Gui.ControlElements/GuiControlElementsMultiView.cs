using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class GuiControlElementsMultiView : Gui.Core.ViewBase {
        [SerializeField] public MonoControlElementsMobileController ContentLandscape;
        [SerializeField] public MonoControlElementsMobileController ContentPortrait;
    }
}