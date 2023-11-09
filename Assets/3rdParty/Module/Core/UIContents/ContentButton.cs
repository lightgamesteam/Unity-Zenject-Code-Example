using System.Linq;
using Module.Core.UIComponent;
using UnityEngine.Events;

namespace Module.Core.UIContent {
    public class ContentButton {
        protected readonly ComponentButton[] Buttons;

        public ContentButton(params ComponentButton[] componentButtons) {
            Buttons = componentButtons ?? new ComponentButton[0];
        }

        public bool GetInteractable() {
            var isNotValid = Buttons.Aggregate(false, (current, componentButton) => current | !componentButton.Interactable);
            return !isNotValid;
        }

        public void SetInteractable(bool isActive) {
            foreach (var componentButton in Buttons) {
                componentButton.Interactable = isActive;
            }
        }

        public void AddListener(UnityAction call) {
            foreach (var componentButton in Buttons) {
                componentButton.AddListener(call);
            }
        }

        public void RemoveListener(UnityAction call) {
            foreach (var componentButton in Buttons) {
                componentButton.RemoveListener(call);
            }
        }

        public void RemoveAllListeners() {
            foreach (var componentButton in Buttons) {
                componentButton.RemoveAllListeners();
            }
        }
    }
}