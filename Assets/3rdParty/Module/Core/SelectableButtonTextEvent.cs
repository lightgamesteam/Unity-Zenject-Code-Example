using Module.Core.Attributes;
using UnityEngine;

namespace Module.Core {
    public class SelectableButtonTextEvent : MonoBehaviour {
        [ShowOnly][SerializeField] public string DisplayText;
    }
}
