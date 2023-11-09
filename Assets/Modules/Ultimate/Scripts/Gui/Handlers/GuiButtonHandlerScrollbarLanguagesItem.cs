using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonHandlerScrollbarLanguagesItem : GuiButtonHandler {
        [Inject] private readonly ILanguageHandler _managerLanguageHandler = default;
        
        private int _id;

        public void Initialize(int id) {
            _id = id;
        }
        
        protected override void SendSignal() {
            _managerLanguageHandler.SetLanguage(_id);
        }
    }
}