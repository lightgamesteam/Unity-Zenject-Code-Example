using UnityEngine;

namespace Module.Swipe.Devices {
    public class SwipeMouseAndTouch : SwipeBase {
        protected override int Tolerance => Input.touchCount > 0 ? GetTouchTolerance() : GetMouseTolerance();

        protected override void OnInput() {
            if (Input.touchCount > 0) {
                OnInputTouch();
            } else {
                OnInputMouse();
            }
        }

        private static int GetMouseTolerance() {
            return 50;
        }

        private static int GetTouchTolerance() {
            return Mathf.RoundToInt(Screen.dpi / 100 * 8);
        }

        private void OnInputMouse() {
            if (Input.GetMouseButtonDown(0)) {
                OnPhaseBegan(Input.mousePosition);
            } else if (Input.GetMouseButton(0)) {
                OnPhaseMoved(Input.mousePosition);
            } else if (Input.GetMouseButtonUp(0)) {
                OnPhaseEnded();
            }
        }

        private void OnInputTouch() {
            if (Input.touchCount > 0) {
                var touch = Input.touches[0];
                switch (touch.phase) {
                    case TouchPhase.Began:
                        OnPhaseBegan(touch.position);
                        break;
                    case TouchPhase.Moved:
                        OnPhaseMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                        OnPhaseEnded();
                        break;
                }
            }
        }
    }
}
