using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module.Core.UIComponent {
    public class ComponentButtonTriggers : Selectable, IPointerClickHandler, ISubmitHandler {
        #region Variables

        [Header("Trigger properties")]
        [SerializeField] private Image _image;

        private bool _isPressed;
        private float _deltaTime;
        private const float DELAY = 0.05f;

        private event Action OnPointerClickEvent;
        private event Action OnPointerDownEvent;
        private event Action OnPointerUpEvent;

        #endregion

        #region Unity methods

        private void FixedUpdate() {
            if (_isPressed) {
                if (_deltaTime >= DELAY) {
                    _deltaTime -= DELAY;
                    Invoke();
                }
                _deltaTime += Time.fixedDeltaTime;
            }
        }

        #endregion

        #region Public methods

        public bool Interactable { get => interactable; set => interactable = value; }

        public void AddListener(Action call) {
            OnPointerClickEvent += call;
        }

        public void RemoveListener(Action call) {
            OnPointerClickEvent -= call;
        }

        public void Invoke() {
            OnPointerClickEvent?.Invoke();
        }

        public void RemoveAllListeners() { 
            OnPointerClickEvent = null;
        }

        public void AddPointerDownListener(Action call) {
            OnPointerDownEvent += call;
        }

        public void RemovePointerDownListener(Action call) {
            OnPointerDownEvent -= call;
        }

        public void RemovePointerDownAllListeners() { 
            OnPointerDownEvent = null;
        }

        public void AddPointerUpListener(Action call) {
            OnPointerUpEvent += call;
        }

        public void RemovePointerUpListener(Action call) {
            OnPointerUpEvent -= call;
        }

        public void RemovePointerUpAllListeners() { 
            OnPointerUpEvent = null;
        }

        #endregion

        #region IPointerHandlers

        public override void OnPointerDown(PointerEventData eventData) {
            base.OnPointerDown(eventData);
            _image.transform.localScale = Vector3.one * .9f;
            _deltaTime = 0f;
            _isPressed = true;
            OnPointerDownEvent?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData) {
            base.OnPointerUp(eventData);
            _image.transform.localScale = Vector3.one;
            _isPressed = false;
            OnPointerUpEvent?.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData) {
            Press();
            if (!IsActive() || !IsInteractable()) { return; }

            DoStateTransition(SelectionState.Pressed, false);
        }

        #endregion

        private void Press() {
            _image.transform.localScale = Vector3.one;
            _isPressed = false;
            Invoke();
        }
    }
}