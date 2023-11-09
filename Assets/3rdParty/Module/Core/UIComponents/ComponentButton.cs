using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module.Core.UIComponent {
    [Serializable]
    public class ComponentButton {
        #region Inspector

        [SerializeField] private Button _button;

        #endregion

        #region Variables

        /// <summary> <para>UI.Selectable.interactable.</para> </summary>
        public bool Interactable {
            get { return _button.interactable; }
            set { _button.interactable = value; }
        }

        #endregion

        #region Public methods

        /// <summary> <para>Add a non persistent listener to the UnityEvent.</para> </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction call) {
            _button.onClick.AddListener(call);
        }

        /// <summary> <para>Remove a non persistent listener from the UnityEvent.</para> </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction call) {
            _button.onClick.RemoveListener(call);
        }

        /// <summary> <para>Remove all non-persisent listeners from the event.</para> </summary>
        public void RemoveAllListeners() {
            _button.onClick.RemoveAllListeners();
        }

        public void SetActive(bool isActive)
        {
            _button.gameObject.SetActive(isActive);
        }

        /// <summary> <para>Invoke all registered callbacks (runtime and persistent).</para> </summary>
        public void Invoke() {
            _button.onClick.Invoke();
        }

        #endregion
    }
}