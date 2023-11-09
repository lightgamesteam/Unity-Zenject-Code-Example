using System;
using Module.Engine.ScrollbarLayers.Content;
using UnityEngine;

namespace Module.Engine.ScrollbarLayers.Component {
    [Serializable]
    public class ComponentScrollbarLayersAutoRotationView {
        [SerializeField] public ContentScrollbarLayers Landscape;
        [SerializeField] public ContentScrollbarLayers Portrait;
    }
}
