using TDL.Modules.Ultimate.GuiControlElements;
using Zenject;

namespace TDL.Modules.Ultimate.States {
    public class FsmStateUltimateMainStandalone : FsmStateUltimate {
        [Inject] private readonly GuiControlElementsSimpleController _guiControlElementsSimple = default;
        [Inject] private readonly AsyncProcessorService _asyncProcessor = default;
        
        public override void EnterState() {
            base.EnterState();
            _guiControlElementsSimple.PanelAdditional.View.LayersToggle.IsOn = true;
            _asyncProcessor.Wait(0, _guiControlElementsSimple.PanelAdditional.View.LayersToggle.Invoke);
            _asyncProcessor.Wait(0, _guiControlElementsSimple.PanelScene.View.ResetButton.Invoke);
        }
    }
}