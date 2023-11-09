using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module.Core.UIComponent {
    [Serializable]
    public class ComponentToggle {
        #region Inspector

        [SerializeField] protected Toggle Toggle;

        #endregion

        #region Variables

        /// <summary> <para>UI.Selectable.interactable.</para> </summary>
        public bool Interactable {
            get => Toggle.interactable;
            set => Toggle.interactable = value;
        }

        /// <summary> <para>Return or set whether the Toggle is on or not.</para> </summary>
        public bool IsOn {
            get => Toggle.isOn;
            set => Toggle.isOn = value;
        }

        #endregion

        #region Public methods

        /// <summary> <para>Add a non persistent listener to the UnityEvent.</para> </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<bool> call) {
            Toggle.onValueChanged.AddListener(call);
        }

        /// <summary> <para>Remove a non persistent listener from the UnityEvent.</para> </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<bool> call) {
            Toggle.onValueChanged.RemoveListener(call);
        }

        /// <summary> <para>Remove all non-persisent listeners from the event.</para> </summary>
        public void RemoveAllListeners() {
            Toggle.onValueChanged.RemoveAllListeners();
        }

        /// <summary> <para>Invoke all registered callbacks (runtime and persistent).</para> </summary>
        public void Invoke() {
            Toggle.onValueChanged.Invoke(Toggle.isOn);
        }

        public Toggle GetToggle()
        {
            return Toggle;
        }

        #endregion
    }
}