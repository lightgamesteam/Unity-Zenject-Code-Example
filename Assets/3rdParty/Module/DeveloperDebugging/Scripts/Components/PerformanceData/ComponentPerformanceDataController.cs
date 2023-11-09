using Module.DeveloperDebugging.Core;

namespace Module.DeveloperDebugging.Components.PerformanceData {
    public class ComponentPerformanceDataController : ComponentControllerBase<ComponentPerformanceDataView> {
        protected override void Initialize() {
            base.Initialize();
            View.ComponentFPSCounter.Initialize();
            View.ComponentMemoryCounter.Initialize();
        }

        protected override void Release() {
            View.ComponentFPSCounter.Release();
            View.ComponentMemoryCounter.Release();
            base.Release();
        }
    }
}
