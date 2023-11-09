using System;
using TMPro;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbar {
    [Serializable]
    public abstract class ItemScrollbarViewBase : Gui.Core.ViewBase {
        [SerializeField] public TextMeshProUGUI DisplayText;
        [Header("State.Item")]
        [SerializeField] public CanvasGroup ItemStateCanvasGroup;
        [Header("State.Select")]
        [SerializeField] public CanvasGroup SelectStateNoneCanvasGroup;
        [SerializeField] public CanvasGroup SelectStateActiveCanvasGroup;
    }
}