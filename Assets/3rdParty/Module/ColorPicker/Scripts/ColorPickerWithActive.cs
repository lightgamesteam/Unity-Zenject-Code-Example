using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module.ColorPicker {
    public class ColorPickerWithActive : ColorPicker {
        #region Variables

        [SerializeField] protected Toggle ActiveToggle;
        [SerializeField] protected TextMeshProUGUI ActiveText;

        private event UnityAction<bool> OnSetActiveEvent;

        #endregion

        #region Unity methods

        protected override void Awake() {
            base.Awake();
            ActiveToggle.onValueChanged.AddListener(OnSetActive);
        }

        #endregion

        #region Public methods

        public void SetActiveText(string value) {
            ActiveText.text = value;
        }
        
        public bool GetActive() {
            return ActiveToggle.isOn;
        }

        public void SetActive(bool isOn) {
            ActiveToggle.isOn = isOn;
        }
        
        public void AddOnSetActiveListener(UnityAction<bool> active) {
            OnSetActiveEvent += active;
        }

        public void RemoveOnSetActiveListener(UnityAction<bool> active) {
            if (OnSetActiveEvent != null) {
                OnSetActiveEvent -= active;
            }
        }

        #endregion

        private void OnSetActive(bool isOn) {
            OnSetActiveEvent?.Invoke(isOn);
        }
    }
}
