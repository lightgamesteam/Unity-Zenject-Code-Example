using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonHandlerScrollbarLabelsItem : GuiButtonHandler {
        [Inject] private readonly ILabelHandler _managerLabelHandler = default;
        private int _id;

        public void Initialize(int id) {
            _id = id;
        }
        
        protected override void SendSignal() {
            _managerLabelHandler.SetModel(_id);
        }
    }
}