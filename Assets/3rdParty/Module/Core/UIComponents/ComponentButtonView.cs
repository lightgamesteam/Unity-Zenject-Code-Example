using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module.Core.UIComponent {
    public class ComponentButtonView : Button {
        private float _lastClick;
        private const float INTERVAL = 0.4f;

        private event UnityAction OnPointerSingleClickEvent;

        public bool IsMultiClick { get; set; }

        public override void OnPointerClick(PointerEventData eventData) {
            if (!IsMultiClick) {
                base.OnPointerClick(eventData);
            } else {
                if (_lastClick + INTERVAL <= Time.time) {
                    OnPointerSingleClickEvent?.Invoke();
                } else {
                    base.OnPointerClick(eventData);
                }
                _lastClick = Time.time;
            }
        }

        #region Public methods

        public void AddSingleClickListener(UnityAction call) { OnPointerSingleClickEvent += call; }
        public void RemoveSingleClickListener(UnityAction call) { OnPointerSingleClickEvent -= call; }
        public void RemoveAllSingleClickListeners() { OnPointerSingleClickEvent = null; }
        public void InvokeSingleClick() { OnPointerSingleClickEvent?.Invoke(); }

        #endregion
    }
}
