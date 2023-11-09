using UnityEngine;

namespace Module.Core.Component {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ComponentBase : ControllerBase, IScreenOrientationChange {
        #region Variables

        public RectTransform rectTransform => GetComponent(ref _rectTransform);
        public CanvasGroup canvasGroup => GetComponent(ref _canvasGroup);

        public bool Interactable {
            get => canvasGroup.interactable;
            set => canvasGroup.interactable = value;
        }

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private readonly Vector2 ANCHOR_MIN_DEFAULT_INITIALIZE = Vector2.zero;
        private readonly Vector2 ANCHOR_MAX_DEFAULT_INITIALIZE = Vector2.one;
        private readonly Vector2 ANCHOR_MIN_DEFAULT_RELEASE = new Vector2(1, 0);
        private readonly Vector2 ANCHOR_MAX_DEFAULT_RELEASE = new Vector2(2, 1);

        #endregion

        #region Public methods

        public virtual void ScreenOrientationToLandscape() {}

        public virtual void ScreenOrientationToPortrait() {}

        public void SetStateComponent(bool isActive) {
            if (isActive) {
                ShowComponent();
            } else {
                HideComponent();
            }
        }

        public void ShowComponent() {
            SetVisibility(true);
        }

        public void HideComponent() {
            SetVisibility(false);
        }

        #endregion

        #region Protected methods

        protected override void Initialize() {
            SetAnchor(ANCHOR_MIN_DEFAULT_INITIALIZE, ANCHOR_MAX_DEFAULT_INITIALIZE);
        }

        protected override void Release() {
            SetVisibility(false);
            SetAnchor(ANCHOR_MIN_DEFAULT_RELEASE, ANCHOR_MAX_DEFAULT_RELEASE);
        }

        protected void SetVisibility(bool isVisible) {
            if (canvasGroup != null) {
                canvasGroup.interactable = isVisible;
                canvasGroup.blocksRaycasts = isVisible;
                canvasGroup.alpha = isVisible ? 1f : 0f;
            }
            if (isVisible) {
                SetAnchor(ANCHOR_MIN_DEFAULT_INITIALIZE, ANCHOR_MAX_DEFAULT_INITIALIZE);
            } else {
                SetAnchor(ANCHOR_MIN_DEFAULT_RELEASE, ANCHOR_MAX_DEFAULT_RELEASE);
            }
        }

        #endregion

        #region Private methods

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
