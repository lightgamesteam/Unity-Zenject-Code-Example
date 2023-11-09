using System;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Core.UIComponent {
    [Serializable]
    public class ComponentButtonWithMultiClick {
        #region Inspector

        [SerializeField] protected ComponentButtonView Button;

        #endregion

        #region Public methods

        public bool Interactable { get => Button.interactable; set => Button.interactable = value; }
        public bool IsMultiClick { get => Button.IsMultiClick; set => Button.IsMultiClick = value; }

        public void AddListener(UnityAction call) { Button.onClick.AddListener(call); }
        public void RemoveListener(UnityAction call) { Button.onClick.RemoveListener(call); }
        public void RemoveAllListeners() { Button.onClick.RemoveAllListeners(); }
        public void Invoke() { Button.onClick.Invoke(); }

        public void AddSingleClickListener(UnityAction call) { Button.AddSingleClickListener(call); }
        public void RemoveSingleClickListener(UnityAction call) { Button.RemoveSingleClickListener(call); }
        public void RemoveAllSingleClickListeners() { Button.RemoveAllSingleClickListeners(); }
        public void InvokeSingleClick() { Button.InvokeSingleClick(); }

        #endregion
    }
}