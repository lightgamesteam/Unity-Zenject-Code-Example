using System;
using System.Linq;
using UnityEngine;

namespace Module.Swipe {
    public abstract class SwipeBase : ISwipeResult, ISwipePhaseListeners, ISwipeControllerListeners {
        #region Variables

        private Vector2 _startPosition;
        private Vector2 _endPosition;
        private SwipeMovementTypes _movementType;

        private event Action<ISwipeResult> PhaseBeganEvent;
        private event Action<ISwipeResult> PhaseEndedEvent;
        private event Action<ISwipeResult> PhaseMovedEvent;

        private event Action OnInputEvent;

        #endregion

        #region Interfaces

        #region ISwipeResult

        public Vector2 MovementStartPosition { get; private set; }
        public Vector2 MovementEndPosition { get; private set; }
        public SwipeMovementTypes MovementType { get; private set; }

        #endregion

        #region ISwipePhaseListeners

        void ISwipePhaseListeners.AddPhaseBeganListener(Action<ISwipeResult> action) {
            AddListener(ref PhaseBeganEvent, action);
        }

        void ISwipePhaseListeners.AddPhaseEndedListener(Action<ISwipeResult> action) {
            AddListener(ref PhaseEndedEvent, action);
        }

        void ISwipePhaseListeners.AddPhaseMovedListener(Action<ISwipeResult> action) {
            AddListener(ref PhaseMovedEvent, action);
        }

        void ISwipePhaseListeners.RemovePhaseBeganListener(Action<ISwipeResult> action) {
            RemoveListener(ref PhaseBeganEvent, action);
        }

        void ISwipePhaseListeners.RemovePhaseEndedListener(Action<ISwipeResult> action) {
            RemoveListener(ref PhaseEndedEvent, action);
        }

        void ISwipePhaseListeners.RemovePhaseMovedListener(Action<ISwipeResult> action) {
            RemoveListener(ref PhaseMovedEvent, action);
        }

        void ISwipePhaseListeners.ClearAllPhaseBeganListener() {
            ClearAllListener(ref PhaseBeganEvent);
        }

        void ISwipePhaseListeners.ClearAllPhaseEndedListener() {
            ClearAllListener(ref PhaseEndedEvent);
        }

        void ISwipePhaseListeners.ClearAllPhaseMovedListener() {
            ClearAllListener(ref PhaseMovedEvent);
        }

        #endregion

        #region ISwipeControllerListeners

        void ISwipeControllerListeners.InvokeController() {
            OnInputEvent?.Invoke();
        }

        void ISwipeControllerListeners.AddControllerListener() {
            OnInputEvent = OnInput;
        }

        void ISwipeControllerListeners.RemoveControllerListener() {
            OnInputEvent = null;
        }

        #endregion

        #endregion

        #region Protected methods

        protected virtual int Tolerance { get; set; }

        protected virtual void OnInput() {}

        #region Phases

        protected void OnPhaseBegan(Vector2 position) {
            _startPosition = position;
            _endPosition = position;
            _movementType = SwipeMovementTypes.NONE;
            MovementStartPosition = _startPosition;
            MovementEndPosition = _endPosition;
            MovementType = _movementType;
            InvokeListener(ref PhaseBeganEvent, this);
        }

        protected void OnPhaseMoved(Vector2 position) {
            //TODO Improve the calculation of the swipe.
            //TODO Make the difference between the start and the end point and determine the degree (if diagonal)

            _endPosition = position;
            MovementEndPosition = _endPosition;

            var horDifferenceEndStart = _endPosition.x - _startPosition.x;
            var verDifferenceEndStart = _endPosition.y - _startPosition.y;
            if (Mathf.Abs(horDifferenceEndStart) > Tolerance || Mathf.Abs(verDifferenceEndStart) > Tolerance) {
                var horDistanceStartEnd = Mathf.Abs(_startPosition.x - _endPosition.x);
                var verDistanceStartEnd = Mathf.Abs(_startPosition.y - _endPosition.y);
                if (horDistanceStartEnd > verDistanceStartEnd) {
                    _movementType = horDifferenceEndStart > 0 ? SwipeMovementTypes.RIGHT : SwipeMovementTypes.LEFT;
                } else {
                    _movementType = verDifferenceEndStart > 0 ? SwipeMovementTypes.UP : SwipeMovementTypes.DOWN;
                }
                _startPosition = position;
                MovementType = _movementType;
            }
            InvokeListener(ref PhaseMovedEvent, this);
        }

        protected void OnPhaseEnded() {
            _startPosition = Vector2.zero;
            _endPosition = Vector2.zero;
            _movementType = SwipeMovementTypes.NONE;
            InvokeListener(ref PhaseEndedEvent, this);
        }

        #endregion

        #endregion

        #region Private methods

        private static void InvokeListener(ref Action<ISwipeResult> phaseEvent, ISwipeResult result) {
            phaseEvent?.Invoke(result);
        }

        private static void AddListener(ref Action<ISwipeResult> phaseEvent, Action<ISwipeResult> action) {
            if (phaseEvent == null || phaseEvent != null && !phaseEvent.GetInvocationList().Contains(action)) {
                phaseEvent += action;
            }
        }

        private static void RemoveListener(ref Action<ISwipeResult> phaseEvent, Action<ISwipeResult> action) {
            if (phaseEvent != null && phaseEvent.GetInvocationList().Contains(action)) {
                phaseEvent -= action;
            }
        }

        private static void ClearAllListener(ref Action<ISwipeResult> phaseEvent) {
            if (phaseEvent != null) {
                phaseEvent = null;
            }
        }

        #endregion
    }
}
