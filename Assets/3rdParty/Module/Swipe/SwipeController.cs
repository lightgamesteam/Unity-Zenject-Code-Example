using Module.Swipe.Devices;
using Module.Swipe.Directions;
using UnityEngine;

namespace Module.Swipe {
    public class SwipeController : MonoBehaviour {
        #region Variables

        public readonly SwipeLeft SwipeLeft = new SwipeLeft();
        public readonly SwipeRight SwipeRight = new SwipeRight();

        protected readonly SwipeBase SwipeBase = new SwipeMouseAndTouch();

        #endregion

        #region Unity method

        private void Start() {
            ((ISwipePhaseListeners)SwipeBase).AddPhaseEndedListener(SwipeLeft.SwipeProcess);
            ((ISwipePhaseListeners)SwipeBase).AddPhaseEndedListener(SwipeRight.SwipeProcess);
        }

        private void OnEnable() {
            ((ISwipeControllerListeners)SwipeBase).AddControllerListener();
        }

        private void OnDisable() {
            ((ISwipeControllerListeners)SwipeBase).RemoveControllerListener();
        }

        private void Update() {
            ((ISwipeControllerListeners)SwipeBase).InvokeController();
        }

        #endregion
    }
}
