using System;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    [Serializable]
    public class ItemScrollbarLayersViewLabel : ItemScrollbarLayersViewBase {
        [Header("State.Eye")]
        [SerializeField] public CanvasGroup EyeStateNoneCanvasGroup;
        [SerializeField] public CanvasGroup EyeStateShowCanvasGroup;
        [SerializeField] public CanvasGroup EyeStateHideCanvasGroup;
    }
}