using Module.Core.UIComponent;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    [RequireComponent(typeof(ComponentButtonTriggers))]
    public abstract class GuiButtonTriggerHandler : MonoBehaviour {
        [Inject] protected readonly SignalBus SignalBus;

        private void Awake() {
            var button = GetComponent<ComponentButtonTriggers>();
            Assert.IsNotNull(button);
            button.AddListener(SendSignal);
        }

        protected abstract void SendSignal();
    }
}