using System.Collections;
using Module.DeveloperDebugging.Core;
using UnityEngine;

namespace Module.DeveloperDebugging.Components.PerformanceData.FPSCounter {
    public class ComponentFPSCounter : ComponentBase<ComponentFPSCounterView, ComponentFPSCounterController> {
        public override void Activate(MonoBehaviour monoBehaviour, IEnumerator enumerator = null) {
            base.Activate(monoBehaviour, Controller.UpdateCounter());
        }
    }
}
