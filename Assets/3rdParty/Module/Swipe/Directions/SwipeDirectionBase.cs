using System;
using System.Linq;

namespace Module.Swipe.Directions {
    public abstract class SwipeDirectionBase : ISwipeProcess {
        #region Variables

        protected event Action SwipeEvent;

        #endregion

        #region ISwipeProcess

        public virtual void SwipeProcess(ISwipeResult result) { }

        #endregion

        #region Public methods

        public void AddListener(Action action) {
            if (SwipeEvent == null || SwipeEvent != null && !SwipeEvent.GetInvocationList().Contains(action)) {
                SwipeEvent += action;
            }
        }

        public void RemoveListener(Action action) {
            if (SwipeEvent != null && SwipeEvent.GetInvocationList().Contains(action)) {
                SwipeEvent -= action;
            }
        }

        public void ClearAllListener() {
            if (SwipeEvent != null) {
                SwipeEvent = null;
            }
        }

        #endregion

        #region Protected methods

        protected void InvokeListener() {
            SwipeEvent?.Invoke();
        }

        #endregion
    }
}
