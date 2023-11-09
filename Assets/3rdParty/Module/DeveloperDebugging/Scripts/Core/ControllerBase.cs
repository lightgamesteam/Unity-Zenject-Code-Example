using UnityEngine;

namespace Module.DeveloperDebugging.Core {
    public abstract class ControllerBase : MonoBehaviour, IInitializable, IReleasable {
        public virtual void Initialize() {}
        public virtual void Release() {}

        void IInitializable.Initialize() { Initialize(); }
        void IReleasable.Release() { Release(); }
    }
}
