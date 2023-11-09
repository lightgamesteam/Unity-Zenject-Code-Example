using Module.Core;
using Module.Core.Attributes;
using Module.Core.UIComponent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Engine.ScrollbarLabels.Item {
    public abstract class ItemScrollbarLabelsPartViewBase : MonoBehaviour {
        #region Variables

        [SerializeField] protected TextMeshProUGUI DisplayText;
        [SerializeField] protected ComponentButton SelectButton;
        [LabelOverride("Light.State -> None")]
        [SerializeField] protected CanvasGroup StateNoneCanvasGroup;
        [LabelOverride("Light.State -> Active")]
        [SerializeField] protected CanvasGroup StateActiveCanvasGroup;

        #endregion

        public virtual void Initialize(string displayText, UnityAction action) {
            DisplayText.text = displayText;
            SelectButton.AddListener(action);
            SetLightState(LightStateType.Disable);
        }

        public virtual void SetLightState(LightStateType state) {
            switch (state) {
                case LightStateType.Disable: {
                    Utilities.Component.SetActiveCanvasGroup(StateNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(StateActiveCanvasGroup, false);
                    break;
                }
                case LightStateType.Enable: {
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
