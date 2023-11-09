using UnityEngine;

namespace Module.Swipe {
    public class SwipeTouch : SwipeBase {
        protected override int Tolerance => Mathf.RoundToInt(Screen.dpi / 100 * 8);

        protected override void OnInput() {
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
