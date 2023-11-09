using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Gui.Core.UIMulti {
    [Serializable]
    public class MultiButton {
        private readonly List<Button> _buttons;
        
        public MultiButton(params Button[] buttons) {
            _buttons = new List<Button>(buttons);
        }
        
        public bool Contains(Button button) {
            return _buttons.Contains(button);
        }

        public void Invoke(bool isFirst = true) {
            if (isFirst) {
                _buttons[0].onClick.Invoke();
            } else {
                foreach (var button in _buttons) {
                    button.onClick.Invoke();
                }
            }
        }
    }
}