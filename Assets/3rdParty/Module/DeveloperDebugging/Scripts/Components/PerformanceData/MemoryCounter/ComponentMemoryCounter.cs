using System.Collections;
using Module.DeveloperDebugging.Core;
using UnityEngine;

namespace Module.DeveloperDebugging.Components.PerformanceData.MemoryCounter {
    public class ComponentMemoryCounter : ComponentBase<ComponentMemoryCounterView, ComponentMemoryCounterController> {
        public override void Activate(MonoBehaviour monoBehaviour, IEnumerator enumerator = null) {
            base.Activate(monoBehaviour, Controller.UpdateCounter());
        }
    }
}
