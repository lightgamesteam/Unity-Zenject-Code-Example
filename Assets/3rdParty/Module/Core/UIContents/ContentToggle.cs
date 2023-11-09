using System;
using System.Linq;
using Module.Core.UIComponent;
using UnityEngine.Events;

namespace Module.Core.UIContent {
    [Serializable]
    public class ContentToggle {
        protected readonly ComponentToggle[] Toggles;

        public ContentToggle(params ComponentToggle[] componentToggles) {
            Toggles = componentToggles ?? new ComponentToggle[0];
        }

        public bool GetInteractable() {
            var isNotValid = Toggles.Aggregate(false, (current, componentToggle) => current | !componentToggle.Interactable);
            return !isNotValid;
        }

        public void SetInteractable(bool isActive) {
            foreach (var componentToggle in Toggles) {
                componentToggle.Interactable = isActive;
            }
        }

        public bool GetIsOn() {
            var isNotValid = Toggles.Aggregate(false, (current, componentToggle) => current | !componentToggle.IsOn);
            return !isNotValid;
        }

        public void SetIsOn(bool isOn) {
            foreach (var componentToggle in Toggles) {
                componentToggle.IsOn = isOn;
            }
        }

        public void AddListener(UnityAction<bool> call) {
            foreach (var componentToggle in Toggles) {
                componentToggle.AddListener(call);
            }
        }

        public void RemoveListener(UnityAction<bool> call) {
            foreach (var componentToggle in Toggles) {
                componentToggle.RemoveListener(call);
            }
        }

        public void RemoveAllListeners() {
            foreach (var componentToggle in Toggles) {
                componentToggle.RemoveAllListeners();
            }
        }
    }
}