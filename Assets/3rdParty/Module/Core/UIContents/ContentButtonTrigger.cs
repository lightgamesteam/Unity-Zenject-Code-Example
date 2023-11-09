using System;
using System.Linq;
using Module.Core.UIComponent;

namespace Module.Core.UIContent {
    public class ContentButtonTrigger {
        protected readonly ComponentButtonTriggers[] ButtonTriggers;

        public ContentButtonTrigger(params ComponentButtonTriggers[] componentButtonTriggers) {
            ButtonTriggers = componentButtonTriggers ?? new ComponentButtonTriggers[0];
        }

        public void SetActive(bool isActive) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.gameObject.SetActive(isActive);
            }
        }

        public bool GetInteractable() {
            var isNotValid = ButtonTriggers.Aggregate(false, (current, componentButtonTrigger) => current | !componentButtonTrigger.Interactable);
            return !isNotValid;
        }

        public void SetInteractable(bool isActive) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.Interactable = isActive;
            }
        }

        public void AddListener(Action call) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.AddListener(call);
            }
        }

        public void RemoveListener(Action call) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.RemoveListener(call);
            }
        }

        public void RemoveAllListeners() { 
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.RemoveAllListeners();
            }
        }

        public void AddPointerDownListener(Action call) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.AddPointerDownListener(call);
            }
        }

        public void RemovePointerDownListener(Action call) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.RemovePointerDownListener(call);
            }
        }

        public void RemovePointerDownAllListeners() { 
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.RemovePointerDownAllListeners();
            }
        }

        public void AddPointerUpListener(Action call) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.AddPointerUpListener(call);
            }
        }

        public void RemovePointerUpListener(Action call) {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.RemovePointerUpListener(call);
            }
        }

        public void RemovePointerUpAllListeners() {
            foreach (var componentButtonTrigger in ButtonTriggers) {
                componentButtonTrigger.RemovePointerUpAllListeners();
            }
        }
    }
}