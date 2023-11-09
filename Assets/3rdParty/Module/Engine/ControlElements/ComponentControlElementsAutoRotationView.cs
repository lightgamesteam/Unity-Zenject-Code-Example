using System;
using Module.Engine.ControlElements.Content;
using UnityEngine;

namespace Module.Engine.ControlElements.Component {
    [Serializable]
    public class ComponentControlElementsAutoRotationView {
        [SerializeField] public ContentControlElements Landscape;
        [SerializeField] public ContentControlElements Portrait;
    }
}
