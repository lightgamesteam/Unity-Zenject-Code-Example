using Module.Core.UIComponent;
using UnityEngine;

namespace Module.Engine.ControlElements.Content {
    public class ContentMenuModule : MonoBehaviour {
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] public ComponentToggle LanguagesToggle;
        [SerializeField] public ComponentToggle LabelsToggle;
        [SerializeField] public ComponentToggle LayersToggle;
    }
}
