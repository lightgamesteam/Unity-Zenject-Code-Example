using UnityEngine;
using UnityEngine.EventSystems;

namespace Module.ColorPicker {
    public class PaletteGradient : MonoBehaviour, IPointerDownHandler, IDragHandler {
        [SerializeField] protected ColorJoystick ColorJoystick;

        public void OnPointerDown(PointerEventData eventData) {
            Vector2 position;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, null, out position)) {
                return;
            }
            ((IDragHandler)ColorJoystick).OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            ((IDragHandler)ColorJoystick).OnDrag(eventData);
        }
    }
}