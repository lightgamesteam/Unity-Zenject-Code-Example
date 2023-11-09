using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Engine.ScrollbarLabels.Content {
    [Serializable]
    public class ContentScrollbarLabelsView {
        [Header("Content -> Component")]
        [SerializeField] public RectTransform ComponentContent;

        [Header("Content -> Scroll View")]
        [SerializeField] public ScrollRect ScrollRect;
        [SerializeField] public RectTransform ViewportRectTransform;
        [SerializeField] public Transform ScrollContent;

        [Header("Localization")]
        [SerializeField] public TextMeshProUGUI TitleText;
    }
}
