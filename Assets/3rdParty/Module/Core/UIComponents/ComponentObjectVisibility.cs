using UnityEngine;

namespace Module.Core.UIComponent {
    public class ComponentObjectVisibility : MonoBehaviour {
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;

        public void SetVisibility(bool isActive) {
            gameObject.SetActive(isActive);
        }
    }
}
