using System.Collections;
using Module.DeveloperDebugging.Core;
using UnityEngine;

namespace Module.DeveloperDebugging.Components.PerformanceData {
    public class ComponentPerformanceData : ComponentBase<ComponentPerformanceDataView, ComponentPerformanceDataController> {
        public override void Activate(MonoBehaviour monoBehaviour, IEnumerator enumerator = null) {
            View.ComponentFPSCounter.Activate(monoBehaviour);
            View.ComponentMemoryCounter.Activate(monoBehaviour);
        }

        public override void Deactivate(MonoBehaviour monoBehaviour) {
            View.ComponentFPSCounter.Deactivate(monoBehaviour);
            View.ComponentMemoryCounter.Deactivate(monoBehaviour);
        }
    }
}
