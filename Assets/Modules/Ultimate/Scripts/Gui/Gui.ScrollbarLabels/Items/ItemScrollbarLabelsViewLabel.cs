using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public class ItemScrollbarLabelsViewLabel : ItemScrollbarLabelsViewBase {
        [Header("State.Eye")]
        [SerializeField] public CanvasGroup EyeStateNoneCanvasGroup;
        [SerializeField] public CanvasGroup EyeStateShowCanvasGroup;
        [SerializeField] public CanvasGroup EyeStateHideCanvasGroup;
    }
}