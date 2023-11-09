using System;

namespace Module.Core {
    public class EventAction {
        private event Action Event;
        
        public void AddListener(Action action) {
            Event += action;
        }

        public void RemoveListener(Action action) {
            if (Event != null) {
                Event -= action;
            }
        }
        
        public void Invoke() {
            Event?.Invoke();
        }
    }
    
    public class EventAction<T> {
        private event Action<T> Event;
        
        public void AddListener(Action<T> action) {
            Event += action;
        }

        public void RemoveListener(Action<T> action) {
            if (Event != null) {
                Event -= action;
            }
        }
        
        public void Invoke(T value) {
            Event?.Invoke(value);
        }
    }
}