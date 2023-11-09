using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Engine.ScrollbarLayers.Content {
    [Serializable]
    public class ContentScrollbarLayersView {
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
