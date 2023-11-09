using Module.Core.UIComponent;
using UnityEngine;

namespace Module.Engine.ControlElements.Content {
    public class ContentMenuScene : MonoBehaviour {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public ComponentButtonTriggers ZoomPlusButton;
        [SerializeField] public ComponentButtonTriggers ZoomMinusButton;
        [SerializeField] public ComponentToggle ColorPickerToggle;
        [SerializeField] public ComponentButton ResetButton;
    }
}
