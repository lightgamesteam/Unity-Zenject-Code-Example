using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Module.InputHandler {
    public class InputHandler : MonoBehaviour, IPointerHandlers, IDragHandlers {
        #region Variables

        private event Action<PointerEventData> PointerClickEvent;
        private event Action PointerUpEvent;
        private event Action PointerDownEvent;

        private const int MOUSE_ID0 = -1;
        private const int TOUCH_ID0 = 0;

        private bool _isDrag;
        private bool _isTouchOne;

        #endregion

        public bool Interactable { get; set; } = true;

        public void AddPointerClickListener(Action<PointerEventData> action) {
            PointerClickEvent += action;
        }

        public void RemovePointerClickListener(Action<PointerEventData> action) {
            PointerClickEvent -= action;
        }

        public void AddPointerUpListener(Action action) {
            PointerUpEvent += action;
        }

        public void RemovePointerUpListener(Action action) {
            PointerUpEvent -= action;
        }

        public void AddPointerDownListener(Action action) {
            PointerDownEvent += action;
        }

        public void RemovePointerDownListener(Action action) {
            PointerDownEvent -= action;
        }

        #region IPointer handlers

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (!Interactable) { return; }

            if (_isTouchOne && !_isDrag) {
                PointerClickEvent?.Invoke(eventData);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if (!Interactable) { return; }

            PointerUpEvent?.Invoke();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if (!Interactable) { return; }

            PointerDownEvent?.Invoke();
        }

        #endregion

        #region IDrag handlers

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData) {
            if (!Interactable) { return; }

            _isTouchOne = false;
            _isDrag = false;

            if (eventData.pointerId != MOUSE_ID0 && eventData.pointerId != TOUCH_ID0 || Input.touchCount > 1) {
                return;
            }

            _isTouchOne = true;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            if (!Interactable) { return; }

            if (_isTouchOne) {
                _isDrag = true;
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if (!Interactable) { return; }

            if (_isDrag && Input.touchCount > 1) {
                _isDrag = false;
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            if (!Interactable) { return; }

            if (_isDrag && Input.touchCount > 1) {
                _isDrag = false;
            }
        }

        #endregion
    }
}
