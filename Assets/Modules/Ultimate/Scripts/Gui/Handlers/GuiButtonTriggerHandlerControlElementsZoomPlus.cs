using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonTriggerHandlerControlElementsZoomPlus : GuiButtonTriggerHandler {
        [Inject] private readonly SmoothOrbitCam _smoothOrbitCam = default;

        protected override void SendSignal() {
            _smoothOrbitCam.Zoom(Constants.Data.ZOOM_PLUS_VALUE);
        }
    }
}