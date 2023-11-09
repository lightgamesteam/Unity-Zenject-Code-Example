using Module.Core;
using Module.Core.Attributes;
using Module.Core.UIComponent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Engine.ScrollbarLanguages.Item {
    public class ItemScrollbarLanguagesView : MonoBehaviour {
        #region Variables

        [SerializeField] protected TextMeshProUGUI DisplayText;
        [SerializeField] protected ComponentButton SelectButton;
        [LabelOverride("Light.State -> None")]
        [SerializeField] protected CanvasGroup StateNoneCanvasGroup;
        [LabelOverride("Light.State -> Active")]
        [SerializeField] protected CanvasGroup StateActiveCanvasGroup;

        #endregion

        public void Initialize(string displayText, UnityAction action) {
            DisplayText.text = displayText;
            SelectButton.AddListener(action);
            SetLightState(SelectStateType.None);
        }

        public void SetLightState(SelectStateType state) {
            switch (state) {
                case SelectStateType.None: {
                    Utilities.Component.SetActiveCanvasGroup(StateNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(StateActiveCanvasGroup, false);
                    break;
                }
                case SelectStateType.Active: {
                    Utilities.Component.SetActiveCanvasGroup(StateNoneCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(StateActiveCanvasGroup, true);
                    break;
                }
                default: {
                    Utilities.Component.SetActiveCanvasGroup(StateNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(StateActiveCanvasGroup, false);
                    break;
                }
            }
        }
    }
}
