using Module.Common;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonHandlerControlElementsClose : GuiButtonHandler {
        [Inject] private readonly ModuleEntryPoint _moduleEntryPoint = default;

        protected override void SendSignal() {
            _moduleEntryPoint.CloseModule();
        }
    }
}