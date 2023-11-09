using Module.Core.Interfaces;
using UnityEngine;

namespace Module.Core.Content {
    public abstract class MonoControllerBase : MonoBehaviour, IInitializable, IReleasable {
        public virtual void Initialize() {}
        public virtual void Release() {}

        void IInitializable.Initialize() { Initialize(); }
        void IReleasable.Release() { Release(); }
    }
}
