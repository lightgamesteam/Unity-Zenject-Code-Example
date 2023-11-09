using System;
using TMPro;
using UnityEngine;

namespace TDL.Modules.Ultimate.Core {
    [Serializable]
    public class ItemElementLabelView : Gui.Core.ViewBase {
        [SerializeField] public GameObject Content;
        [SerializeField] public TextMeshPro DisplayText;
        [SerializeField] public Transform HeaderTransform;
        [SerializeField] public Transform BodyTransform;
        [SerializeField] public float TextScaleMultiplier = 2.2f;
        [SerializeField] public bool IsMouseEnter;
        [SerializeField] public LineRenderer LineRenderer;
    }
}