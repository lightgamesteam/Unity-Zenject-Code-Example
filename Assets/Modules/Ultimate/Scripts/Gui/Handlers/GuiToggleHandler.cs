using Module;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    [RequireComponent(typeof(Toggle))]
    public abstract class GuiToggleHandler : MonoBehaviour {
        [Inject] protected readonly SignalBus SignalBus = default;
        protected Toggle Toggle { get; private set; }

        private void Awake() {
            Toggle = GetComponent<Toggle>();
            Assert.IsNotNull(Toggle);
            //Toggle.onValueChanged.AddListener(value => DebugLog("onValueChanged:" + value, Toggle.name));
            Toggle.onValueChanged.AddListener(SendSignal);
        }

        protected abstract void SendSignal(bool isOn);
        
        private void DebugLog(string title, string infoName) {
            this.Log(title, infoName, Color.gray, Color.black, Color.gray);
        }
    }
}