using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.DeveloperDebugging.Components.Console {
    [Serializable]
    public class ComponentConsoleView {
        [SerializeField] public Button ClearButton;
        [SerializeField] public TMP_InputField ConsoleInputField;
    }
}
