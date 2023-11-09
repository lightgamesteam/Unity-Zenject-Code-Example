using Module.Core.Interfaces;
using UnityEngine;

namespace Module.Core {
    public abstract class ControllerBase : MonoBehaviour, IInitializable, IReleasable {
        protected virtual void Initialize() {}
        protected virtual void Release() {}

        void IInitializable.Initialize() { Initialize(); }
        void IReleasable.Release() { Release(); }
    }
}
