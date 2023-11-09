using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Module.Core {
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(SelectableButtonTextEvent))]
    public class SelectableButtonTooltipEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        private TooltipPanel _tooltipPanel;
        private RectTransform _rectTransform;
        private SelectableButtonTextEvent _selectableButtonTextEvent;

        #region Unity methods
        
        private void Start() {
            if (!_tooltipPanel) {
                _tooltipPanel = gameObject.GetFirstInScene<TooltipPanel>();
            }
            if (!_rectTransform) {
                _rectTransform = gameObject.GetComponent<RectTransform>();
            }
            if (!_selectableButtonTextEvent) {
                _selectableButtonTextEvent = gameObject.GetComponent<SelectableButtonTextEvent>();
            }
        }
        
        private void OnDisable() {
            _tooltipPanel?.SetDisableTooltip();
        }
        
        #endregion

        #region IPointerEnterHandler, IPointerExitHandler
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if (_tooltipPanel != null && _rectTransform != null && _selectableButtonTextEvent != null) {
                _tooltipPanel.SetEnableTooltip(_selectableButtonTextEvent.DisplayText);
                StartCoroutine(IsInside()); 
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            _tooltipPanel?.SetDisableTooltip();
        }
        
        #endregion

        private IEnumerator IsInside() {
            while (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, Input.mousePosition, _rectTransform.GetMyCanvas().worldCamera)) {
                yield return new WaitForEndOfFrame();
            }
            _tooltipPanel.SetSafeDisableTooltip(_selectableButtonTextEvent.DisplayText);
        }
    }
}
