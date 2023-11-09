using System;
using Module.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module.Core {
    public class SelectableLabelTextEvent : MonoBehaviour, IPointerClickHandler {
        [ShowOnly][SerializeField] protected Text ComponentText;
        [ShowOnly][SerializeField] protected TextMeshProUGUI ComponentTextMeshPro;

        public static Action<string> OnClick = delegate { };

        protected Vector2 SelectablePosition;
        private const float SELECTABLE_DISTANCE = 0.2f;

        #region IPointerClickHandler

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            OnClick.Invoke(GetText());
        }

        #endregion

        #region Unity methods

        private void OnEnable() {
            if (ComponentText == null) {
                ComponentText = GetComponent<Text>();
            }
            if (ComponentTextMeshPro == null) {
                ComponentTextMeshPro = GetComponent<TextMeshProUGUI>();
            }
        }

        private void OnMouseDown() {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            if (Input.touchCount > 0) {
                if (Input.touchCount == 1) {
                    SelectablePosition = Input.GetTouch(0).position;
                }
            } else {
                SelectablePosition = Input.mousePosition;
            }
        }

        private void OnMouseUpAsButton() {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            if (Input.touchCount > 0) {
                if (Input.touchCount == 1) {
                    if (Vector2.Distance(SelectablePosition, Input.GetTouch(0).position) <= SELECTABLE_DISTANCE) {
                        OnClick.Invoke(GetText());
                    }
                }
            } else {
                if (Vector2.Distance(SelectablePosition, Input.mousePosition) <= SELECTABLE_DISTANCE && Application.isEditor) {
                    OnClick.Invoke(GetText());
                }
            }
        }

        #endregion

        private string GetText() {
            return 
                ComponentText != null ? ComponentText.text : 
                ComponentTextMeshPro != null ? ComponentTextMeshPro.text : 
                string.Empty;
        }
    }
}
