using UnityEngine;

namespace Module.Swipe.Directions {
    public class SwipeRight : SwipeDirectionBase {
        private const float TARGET_PERCENT = .1f;

        public override void SwipeProcess(ISwipeResult result) {
            var targetDistance = TARGET_PERCENT * Camera.main.pixelWidth;

            if (result.MovementStartPosition.x <= targetDistance && result.MovementType == SwipeMovementTypes.RIGHT) {
                InvokeListener();
            }
        }
    }
}
