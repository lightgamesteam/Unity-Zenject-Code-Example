using Module.Core.UIComponent;
using UnityEngine;

namespace Module.Engine.ControlElements.Content {
    public class ContentMenuApplication : MonoBehaviour {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public ComponentButton CloseButton;
        [SerializeField] public ComponentToggle FullscreenToggle;
    }
}
