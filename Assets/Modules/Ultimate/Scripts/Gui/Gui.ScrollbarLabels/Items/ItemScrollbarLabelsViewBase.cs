using System;
using TDL.Modules.Ultimate.GuiScrollbar;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public abstract class ItemScrollbarLabelsViewBase : ItemScrollbarViewBase {
        [SerializeField] public Button SelectButton;
    }
}