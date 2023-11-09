using System;
using Zenject;

namespace Gui.Core {
    [Serializable]
    public abstract class ModelBase : IInitializable, IDisposable {
        public virtual void Initialize() {}
        public virtual void Dispose() {}
    }
}