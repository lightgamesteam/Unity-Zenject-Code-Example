using Module.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Core.UIComponent {
    public enum ProgressStateType {
        None,
        Current,
        Bad,
        Good,
        Perfect
    }

    public class ComponentProgressStep : MonoBehaviour {
        [SerializeField] private Text _numberText;
        [SerializeField] private CanvasGroup _numberCanvasGroup;
        [SerializeField] private CanvasGroup _stateNoneCanvasGroup;
        [SerializeField] private CanvasGroup _stateCurrentCanvasGroup;
        [SerializeField] private CanvasGroup _stateBadCanvasGroup;
        [SerializeField] private CanvasGroup _stateGoodCanvasGroup;
        [SerializeField] private CanvasGroup _statePerfectCanvasGroup;

        public void Initialize(int number) {
            _numberText.text = number.ToString();
            SetState(ProgressStateType.None);
        }

        public void SetState(ProgressStateType state) {
            HideAllCanvasGroup();
            switch (state) {
                case ProgressStateType.None:
                    Utilities.Component.SetActiveCanvasGroup(_stateNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(_numberCanvasGroup, true);
                    break;
                case ProgressStateType.Current:
                    Utilities.Component.SetActiveCanvasGroup(_stateCurrentCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(_numberCanvasGroup, true);
                    break;
                case ProgressStateType.Bad:
                    Utilities.Component.SetActiveCanvasGroup(_stateBadCanvasGroup, true);
                    break;
                case ProgressStateType.Good:
                    Utilities.Component.SetActiveCanvasGroup(_stateGoodCanvasGroup, true);
                    break;
                case ProgressStateType.Perfect:
                    Utilities.Component.SetActiveCanvasGroup(_statePerfectCanvasGroup, true);
                    break;
            }
        }

        private void HideAllCanvasGroup() {
            Utilities.Component.SetActiveCanvasGroup(_numberCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateNoneCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateCurrentCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateBadCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_stateGoodCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(_statePerfectCanvasGroup, false);
        }
    }
}