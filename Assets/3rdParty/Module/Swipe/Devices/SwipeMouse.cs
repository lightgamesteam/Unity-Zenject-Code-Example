using UnityEngine;

namespace Module.Swipe {
    public class SwipeMouse : SwipeBase {
        protected override int Tolerance => 50;

        protected override void OnInput() {
            if (Input.GetMouseButtonDown(0)) {
                OnPhaseBegan(Input.mousePosition);
            } else if (Input.GetMouseButton(0)) {
                OnPhaseMoved(Input.mousePosition);
            } else if (Input.GetMouseButtonUp(0)) {
                OnPhaseEnded();
            }
        }
    }
}
