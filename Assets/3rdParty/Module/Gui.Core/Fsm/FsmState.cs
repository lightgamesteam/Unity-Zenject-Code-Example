using Module;
using Zenject;

namespace Gui.Core.Fsm {
    public abstract class FsmState<T> : FsmStateBase where T : FsmInfo {
        public virtual void EnterState(T initInfo) {
        }
    }
    
    public abstract class FsmState : FsmStateBase {
        public virtual void EnterState() {
            this.LogBlue("EnterState");
        }
    }
    
    public abstract class FsmStateBase {
        protected FsmService Fsm { get; private set; }
        protected SignalBus SignalBus { get; private set; }
        
        public virtual void ExitState() {
            this.LogBlue("ExitState");
        }
    }
    
    public abstract class FsmInfo {
    }
}