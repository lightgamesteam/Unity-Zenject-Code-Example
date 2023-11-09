using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDL.Views
{
    public class AccessibilityViewItem : ViewBase, ISelectableMenuItem
    {
        public Toggle AccessibilityTextToAudio { get; set; }
        public Toggle AccessibilityGrayscale { get; set; }
        public Toggle AccessibilityLabelLines { get; set; }
        public TMP_Dropdown AccessibilityFontSize { get; set; }
        
        public override void InitUiComponents()
        {
            var accessibilityContainer = transform.Get<Transform>("Container");

            AccessibilityTextToAudio = accessibilityContainer.Get<Toggle>("TextToAudio");
            AccessibilityGrayscale = accessibilityContainer.Get<Toggle>("Grayscale");
            AccessibilityLabelLines = accessibilityContainer.Get<Toggle>("LabelLines");
            AccessibilityFontSize = accessibilityContainer.Get<TMP_Dropdown>("AccessibilityFontSize/FontSizeDropdown");

            Signal.Fire(new SaveRightMenuItemViewSignal(LocalizationConstants.RightMenuAccessibilityKey, this));
            Signal.Fire<AccessibilityMenuInitializedViewSignal>();
        }

        public override void SubscribeOnListeners()
        {
            AccessibilityGrayscale.onValueChanged.AddListener(OnAccessibilityGrayscaleChanged);
            AccessibilityLabelLines.onValueChanged.AddListener(OnAccessibilityLabelLinesChanged);
            AccessibilityTextToAudio.onValueChanged.AddListener(OnAccessibilityTextToAudioChanged); 

            AccessibilityFontSize.onValueChanged.AddListener(OnAccessibilityFontSizeChanged);
        }

        public override void UnsubscribeFromListeners()
        {
            AccessibilityTextToAudio.onValueChanged.RemoveAllListeners();
            AccessibilityGrayscale.onValueChanged.RemoveAllListeners();
            AccessibilityLabelLines.onValueChanged.RemoveAllListeners();
            AccessibilityFontSize.onValueChanged.RemoveAllListeners();
        }
        private void OnAccessibilityTextToAudioChanged(bool status)
        {
            Signal.Fire(new AccessibilityTextToAudioClickViewSignal(status));
        }

        private void OnAccessibilityGrayscaleChanged(bool status)
        {
            Signal.Fire(new AccessibilityGrayscaleClickViewSignal(status));
        }
        
        private void OnAccessibilityLabelLinesChanged(bool status)
        {
            Signal.Fire(new AccessibilityLabelLinesClickViewSignal(status));
        }
        
        private void OnAccessibilityFontSizeChanged(int fontSize)
        {
            Signal.Fire(new AccessibilityFontSizeClickViewSignal(fontSize));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void Select()
        {
        }

        public void Deselect()
        {
        }

        public bool IsSelected()
        {
            return false;
        }
    }
}