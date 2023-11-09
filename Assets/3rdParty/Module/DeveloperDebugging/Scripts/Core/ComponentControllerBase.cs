namespace Module.DeveloperDebugging.Core {
    public abstract class ComponentControllerBase<TView> : IComponentViewConnectable<TView>, IInitializable, IReleasable {
        #region Variables

        public TView View;

        #endregion

        #region Interface

        void IComponentViewConnectable<TView>.Connect(TView view) { View = view; }

        void IInitializable.Initialize() { Initialize(); }

        void IReleasable.Release() { Release(); }

        #endregion

        #region Protected methods

        protected virtual void Initialize() {}

        protected virtual void Release() {}

        #endregion
    }
}