using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonHandlerScrollbarLayersItem : GuiButtonHandler {
        [Inject] private readonly ILayerHandler _managerLayerHandler = default;
        private int _id;

        public void Initialize(int id) {
            _id = id;
        }
        
        protected override void SendSignal() {
            _managerLayerHandler.SetModel(_id);
        }
    }
}