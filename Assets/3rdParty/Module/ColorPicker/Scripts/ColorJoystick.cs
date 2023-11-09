using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module.ColorPicker {
    public class ColorJoystick : MonoBehaviour, IDragHandler {
        #region Variables

        [SerializeField] protected Image CenterImage;
        [SerializeField] protected RectTransform ContentRectTransform;

        private event Action<float, float> DragEvent;

        #endregion

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ContentRectTransform, eventData.position, eventData.pressEventCamera, out var position)) {
                return;
            }
            position.x = Mathf.Max(position.x, ContentRectTransform.rect.min.x);
            position.y = Mathf.Max(position.y, ContentRectTransform.rect.min.y);
            position.x = Mathf.Min(position.x, ContentRectTransform.rect.max.x);
            position.y = Mathf.Min(position.y, ContentRectTransform.rect.max.y);
            transform.localPosition = position;

            var x = position.x / ContentRectTransform.rect.width;
            var y = position.y / ContentRectTransform.rect.height;
            OnDragEvent(x, y);
        }

        public void AddOnDragListener(Action<float, float> action) {
            DragEvent += action;
        }

        public void SetColorAndPosition(Color color, float x01, float y01) {
            CenterImage.color = color;
            var x = x01 * ContentRectTransform.rect.width;
            var y = y01 * ContentRectTransform.rect.height;
            if (!(Math.Abs(transform.localPosition.y - x01) < 0.01f)) {
                transform.localPosition = new Vector3(x, y);
            }
        }

        protected virtual void OnDragEvent(float x01, float y01) {
            DragEvent?.Invoke(x01, y01);
        }
    }
}
