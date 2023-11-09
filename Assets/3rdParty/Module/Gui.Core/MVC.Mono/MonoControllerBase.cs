using Module.Core;
using UnityEngine;

namespace Gui.Core {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MonoControllerBase : ControllerBase {
        #region Variables

        public RectTransform rectTransform => GetComponent(ref _rectTransform);
        public CanvasGroup canvasGroup => GetComponent(ref _canvasGroup);

        protected virtual bool UseAnchor => false;
        
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private bool _isVisibility = true;

        #endregion
        
        #region Public methods

        public override void Initialize() {
            base.Initialize();
            SetStateComponent(_isVisibility);
        }
        
        public void SetStateComponent(bool isActive) {
            _isVisibility = isActive;
            SetVisibility(_isVisibility);
        }

        public void ShowComponent() {
            SetStateComponent(true);
        }

        public void HideComponent() {
            SetStateComponent(false);
        }

        #endregion

        #region Private methods

        private void SetVisibility(bool isVisible) {
            Utilities.Component.SetActiveCanvasGroup(canvasGroup, isVisible);

            if (UseAnchor) {
                if (isVisible) {
                    SetAnchor(Vector2.zero, Vector2.one);
                } else {
                    SetAnchor(new Vector2(1, 0), new Vector2(2, 1));
                }
            }
        }
        
        private T GetComponent<T>(ref T component) {
            if (component == null) {
                component = GetComponent<T>();
            }
            return component;
        }

        private void SetAnchor(Vector2 anchorMin, Vector2 anchorMax) {
            if (rectTransform != null) {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
            }
        }

        #endregion
    }
}