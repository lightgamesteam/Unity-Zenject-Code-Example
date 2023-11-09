using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Gui.Core.UIMulti {
    [Serializable]
    public class MultiToggle {
        public bool isOn {
            get => _isOn;
            set {
                foreach (var toggle in _toggles) {
                    toggle.isOn = value;
                }
            }
        }
        
        private readonly List<Toggle> _toggles;
        private bool _isOn;

        public MultiToggle(params Toggle[] toggles) {
            _toggles = new List<Toggle>(toggles);
            if (_toggles.Count != 0) {
                _isOn = _toggles[0].isOn;
            }
            isOn = _isOn;

            foreach (var toggle in _toggles) {
                toggle.onValueChanged.AddListener(Call);
            }
        }

        public bool Contains(Toggle toggle) {
            return _toggles.Contains(toggle);
        }
        
        public void Invoke() {
            Invoke(isOn);
        }

        public void Invoke(bool value) {
            foreach (var toggle in _toggles) {
                toggle.onValueChanged.Invoke(value);
            }
        }

        private void Call(bool value) {
            if (isOn == value) {
                return; //Multi call
            }
            isOn = _isOn = value;
        }
    }
    
    // public class MultiToggleOld {
    //     public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();
    //     public bool isOn {
    //         get => _isOn;
    //         set {
    //             foreach (var toggle in _toggles) {
    //                 toggle.isOn = value;
    //             }
    //         }
    //     }
    //     
    //     private readonly List<Toggle> _toggles;
    //     private bool _isOn;
    //
    //     public MultiToggle(params Toggle[] toggles) {
    //         _toggles = new List<Toggle>(toggles);
    //         if (_toggles.Count != 0) {
    //             _isOn = _toggles[0].isOn;
    //         }
    //         isOn = _isOn;
    //
    //         foreach (var toggle in _toggles) {
    //             toggle.onValueChanged.AddListener(Call);
    //         }
    //     }
    //
    //     public bool Contains(Toggle toggle) {
    //         return _toggles.Contains(toggle);
    //     }
    //     
    //     public void Invoke() {
    //         onValueChanged.Invoke(isOn);
    //     }
    //
    //     public void Invoke(bool value) {
    //         if (isOn != value) {
    //             isOn = value;
    //         } else {
    //             Invoke();
    //         }
    //     }
    //
    //     private void Call(bool value) {
    //         if (isOn == value) {
    //             return; //Multi call
    //         }
    //         isOn = _isOn = value;
    //         Invoke(value);
    //     }
    // }
}