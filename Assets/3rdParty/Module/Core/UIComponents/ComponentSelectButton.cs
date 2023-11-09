using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module.Core.UIComponent {
    public enum SelectStateType {
        None,
        Warning,
        Error,
        Accept
    }

    public class ComponentSelectButton : MonoBehaviour {
        [SerializeField] private Text _labelText;
        [SerializeField] private ComponentButtonWithMultiClick _selectButton;
        [Header("State Bg")]
        [SerializeField] private CanvasGroup _stateBgNoneCanvasGroup;
        [SerializeField] private CanvasGroup _stateBgWarningCanvasGroup;
        [SerializeField] private CanvasGroup _stateBgErrorCanvasGroup;
        [SerializeField] private CanvasGroup _stateBgAcceptCanvasGroup;
        [Header("State Circle")]
        [SerializeField] private CanvasGroup _stateCircleNoneCanvasGroup;
        [SerializeField] private CanvasGroup _stateCircleWarningCanvasGroup;
        [SerializeField] private CanvasGroup _stateCircleErrorCanvasGroup;
        [SerializeField] private CanvasGroup _stateCircleAcceptCanvasGroup;
        [Header("Content")]
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private RectTransform _content;

        public static Action<Text> OnInitializeChangeFontSize = delegate {  };
        
        public void Initialize(string displayText, UnityAction action) {
            _labelText.text = displayText;
            _selectButton.AddListener(action);
            OnInitializeChangeFontSize?.Invoke(_labelText);
            Recalculate();
            SetState(SelectStateType.None);
        }

        public void SwitchColor(bool isSelection) {
            SetState(isSelection ? SelectStateType.Warning : SelectStateType.None);
        }

        public void SetState(SelectStateType state) {
            HideAllCanvasGroup();
            switch (state) {
                case SelectStateType.None:
                    Utilities.Component.SetActiveCanvasGroup(_stateBgNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(_stateCircleNoneCanvasGroup, true);
                    break;
                case SelectStateType.Warning:
                    Utilities.Component.SetActiveCanvasGroup(_stateBgWarningCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(_stateCircleWarningCanvasGroup, true);
                    break;
                case SelectStateType.Error:
                    Utilities.Component.SetActiveCanvasGroup(_stateBgErrorCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(_stateCircleErrorCanvasGroup, true);
                    break;
                case SelectStateType.Accept:
                    Utilities.Component.SetActiveCanvasGroup(_stateBgAcceptCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(_stateCircleAcceptCanvasGroup, true);
                    break;
            }
        }

        private void HideAllCanvasGroup() {
            Utilities.Component.SetActiveCanvasGroup(_stateBgNoneCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateCircleNoneCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateBgWarningCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateCircleWarningCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateBgErrorCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateCircleErrorCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateBgAcceptCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateCircleAcceptCanvasGroup, false);
        }

        private void Recalculate() {
            if (_labelText.preferredHeight > _content.rect.size.y) {
                _layoutElement.preferredHeight = _labelText.preferredHeight + Mathf.Abs(_content.sizeDelta.y) + 1f;
            }
        }
    }
}