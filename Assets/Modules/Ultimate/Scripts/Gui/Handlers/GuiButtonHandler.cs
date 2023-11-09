using Module;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    [RequireComponent(typeof(Button))]
    public abstract class GuiButtonHandler : MonoBehaviour {
        [Inject] protected readonly SignalBus SignalBus = default;

        private void Awake() {
            var button = GetComponent<Button>();
            Assert.IsNotNull(button);
            //button.onClick.AddListener(() => DebugLog("onClick", button.name));
            button.onClick.AddListener(SendSignal);
        }

        protected abstract void SendSignal();
        
        private void DebugLog(string title, string infoName) {
            this.Log(title, infoName, Color.gray, Color.black, Color.gray);
        }
    }
}