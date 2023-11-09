using System;
using Module.Core.UIComponent;
using UnityEngine;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    [Serializable]
    public class ItemScrollbarLayersViewLayer : ItemScrollbarLayersViewBase {
        [Header("Sub")]
        [SerializeField] public ComponentButton SubContentButton;
        [SerializeField] public GameObject SubContentGameObject;
        [SerializeField] public CanvasGroup SubContentHideCanvasGroup;
        [SerializeField] public CanvasGroup SubContentShowCanvasGroup;
    }
}