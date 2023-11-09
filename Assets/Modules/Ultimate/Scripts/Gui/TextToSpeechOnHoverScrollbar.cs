using Gui.Core;
using TDL.Modules.Ultimate.GuiScrollbar;

namespace TDL.Modules.Ultimate.Core {
    public class TextToSpeechOnHoverScrollbar : TextToSpeechOnHoverUI {
        public IItemHandler Handler { get; private set; }
        
        public override void InitUiComponents() {
            base.InitUiComponents();
            var monoControllerBase = gameObject.GetComponentInParent<MonoControllerBase>();
            if (monoControllerBase != null && monoControllerBase is IItemHandler handler) {
                Handler = handler;
            }
        }
    }
}