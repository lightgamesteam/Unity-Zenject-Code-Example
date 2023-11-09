﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    [Serializable]
    public class MonoScrollbarLabelsView : Gui.Core.ViewBase {
        [Header("Content -> Component")]
        [SerializeField] public RectTransform ComponentContent;

        [Header("Content -> Scroll View")]
        [SerializeField] public ScrollRect ScrollRect;
        [SerializeField] public RectTransform ViewportRectTransform;
        [SerializeField] public Transform ScrollContent;

        [Header("Localization")]
        [SerializeField] public TextMeshProUGUI TitleText;
        [SerializeField] public TextMeshProUGUI CloseText;
    }
}
