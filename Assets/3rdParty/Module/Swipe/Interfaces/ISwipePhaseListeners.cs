using System;

namespace Module.Swipe {
    public interface ISwipePhaseListeners {
        void AddPhaseBeganListener(Action<ISwipeResult> action);
        void AddPhaseEndedListener(Action<ISwipeResult> action);
        void AddPhaseMovedListener(Action<ISwipeResult> action);

        void RemovePhaseBeganListener(Action<ISwipeResult> action);
        void RemovePhaseEndedListener(Action<ISwipeResult> action);
        void RemovePhaseMovedListener(Action<ISwipeResult> action);

        void ClearAllPhaseBeganListener();
        void ClearAllPhaseEndedListener();
        void ClearAllPhaseMovedListener();
    }
}