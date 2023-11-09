namespace Module.Core {
    public abstract class StateControllerBase : ControllerBase {
        public virtual void EnterState() {
            this.LogBlue("EnterState");
        }

        public virtual void ExitState() {
            this.LogBlue("ExitState");
        }

        protected T GetState<T>() where T : StateControllerBase {
            return ModuleControllerBase.Instance.Get<T>();
        }

        protected bool TryGetState<T>(out T state) where T : StateControllerBase {
            return ModuleControllerBase.Instance.TryGet(out state);
        }

        protected void ActiveState<T>() where T : StateControllerBase {
            ModuleControllerBase.Instance.ActiveState<T>();
        }
    }
}
