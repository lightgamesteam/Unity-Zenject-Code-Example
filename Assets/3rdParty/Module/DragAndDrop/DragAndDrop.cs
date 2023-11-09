using System;
using Module.Core.Attributes;
using UnityEngine;

namespace Module.DragAndDrop {
    public class DragAndDrop : MonoBehaviour {
        #region Variables

        [ShowOnly][SerializeField]private bool _isDragged;
        private event Action<Vector2> Dragged;
        private event Action<Vector2> Dropped;

        #endregion

        #region Unity methods

        private void Update() {
            if (Input.GetMouseButton(0)) {
                Drag(Input.mousePosition);
            }
            if (Input.GetMouseButtonDown(0)) {
                BeginDrag();
            }
            if (Input.GetMouseButtonUp(0)) {
                EndDrag(Input.mousePosition);
            }
        }

        #endregion

        #region Public methods

        public void OnDragging() {
            _isDragged = true;
        }

        public void StopDragging() {
            _isDragged = false;
        }

        public void AddListenerDragged(Action<Vector2> action) {
            Dragged += action;
        }

        public void RemoveListenerDragged(Action<Vector2> action) {
            if (Dragged != null) {
                Dragged -= action;
            }
        }

        public void AddListenerDropped(Action<Vector2> action) {
            Dropped += action;
        }

        public void RemoveListenerDropped(Action<Vector2> action) {
            if (Dropped != null) {
                Dropped -= action;
            }
        }

        #endregion

        #region Private methods

        private void BeginDrag() {
            _isDragged = true;
        }

        private void Drag(Vector2 position) {
            if (_isDragged) {
                Dragged?.Invoke(position);
            }
        }

        private void EndDrag(Vector2 position) {
            if (_isDragged) {
                _isDragged = false;
                Dropped?.Invoke(position);
            }
        }

        #endregion
    }
}
