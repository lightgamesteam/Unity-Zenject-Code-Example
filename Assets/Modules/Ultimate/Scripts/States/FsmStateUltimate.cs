using Gui.Core.Fsm;

namespace TDL.Modules.Ultimate.States {
    public interface IFsmStateUltimate {}
    public abstract class FsmStateUltimate<T> : FsmState<T>, IFsmStateUltimate where T : FsmInfo {}
    public abstract class FsmStateUltimate : FsmState, IFsmStateUltimate {}
}