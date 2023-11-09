using System;
using UnityEngine;
using UnityEngine.UI;

namespace Module.DeveloperDebugging.Components.PerformanceData.MemoryCounter {
    [Serializable]
    public class ComponentMemoryCounterView {
        [SerializeField] public Text DisplayText;
        [Space]
        [SerializeField] public bool IsPrecise;
    }
}
