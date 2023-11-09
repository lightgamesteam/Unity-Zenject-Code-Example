using TDL.Modules.Ultimate.GuiControlElements;
using Zenject;

namespace TDL.Modules.Ultimate.States {
    public class FsmStateUltimateMainMobile : FsmStateUltimate {
        [Inject] private readonly GuiControlElementsMultiController _guiControlElementsMulti = default;
        [Inject] private readonly AsyncProcessorService _asyncProcessor = default;
        
        public override void EnterState() {
            base.EnterState();

            if (_guiControlElementsMulti != null) {
                //_asyncProcessor.Wait(0, () => { _guiControlElementsMulti.LayersToggles.Invoke(true); });
                _asyncProcessor.Wait(0, () => { _guiControlElementsMulti.ResetButton.Invoke(); });                
            }
        }
    }
}