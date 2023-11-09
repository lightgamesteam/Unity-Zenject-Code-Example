using System;
using TDL.Modules.Ultimate.GuiScrollbar;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    [Serializable]
    public abstract class ItemScrollbarLayersViewBase : ItemScrollbarViewBase {
        [SerializeField] public Button SelectButton;
    }
}