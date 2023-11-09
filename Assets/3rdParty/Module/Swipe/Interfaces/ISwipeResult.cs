using UnityEngine;

namespace Module.Swipe {
    public interface ISwipeResult {
        Vector2 MovementStartPosition { get; }
        Vector2 MovementEndPosition { get; }
        SwipeMovementTypes MovementType { get; }
    }
}
