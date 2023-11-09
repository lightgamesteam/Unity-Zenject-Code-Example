using System;
using Module.DeveloperDebugging.Components.PerformanceData.FPSCounter;
using Module.DeveloperDebugging.Components.PerformanceData.MemoryCounter;
using UnityEngine;

namespace Module.DeveloperDebugging.Components.PerformanceData {
    [Serializable]
    public class ComponentPerformanceDataView {
        [SerializeField] public ComponentFPSCounter ComponentFPSCounter;
        [SerializeField] public ComponentMemoryCounter ComponentMemoryCounter;
    }
}
