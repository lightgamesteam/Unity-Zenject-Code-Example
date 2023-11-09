using System;
using UnityEngine;

namespace Module.Engine.ControlElements.Content {
    [Serializable]
    public class ContentControlElementsView {
        [SerializeField] public ContentMenuApplication MenuApplication;
        [SerializeField] public ContentMenuScene MenuScene;
        [SerializeField] public ContentMenuModule MenuModule;
        [SerializeField] public ContentInfoModule InfoModule;
    }
}
