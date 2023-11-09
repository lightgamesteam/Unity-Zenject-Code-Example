using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDL.Views
{
    public class RecentlyViewedViewItem : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

        private bool _isSelected;

        private Button _recentlyViewedButton;
        private TextMeshProUGUI _recentlyViewedText;

        public override void InitUiComponents()
        {
            _recentlyViewedButton = GetComponent<Button>();
            _recentlyViewedText = GetComponentInChildren<TextMeshProUGUI>();
            
           Signal.Fire(new SaveRightMenuItemViewSignal(LocalizationConstants.RecentlyViewedKey, this));
        }

        public override void SubscribeOnListeners()
        {
            _recentlyViewedButton.onClick.AddListener(OnRecentlyViewedClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _recentlyViewedButton.onClick.RemoveAllListeners();
        }
        
        private void OnRecentlyViewedClick()
        {
            Signal.Fire(new RecentlyViewedClickViewSignal());
        }
        
        public void Select()
        {
            _isSelected = true;
            SetHoverEnter();
        }

        public void Deselect()
        {
            _isSelected = false;
            SetHoverExit();
        }

        public bool IsSelected()
        {
            return _isSelected;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isSelected)
            {
                SetHoverEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isSelected)
            {
                SetHoverExit();
            }
        }
        
        private void SetHoverEnter()
        {
            _recentlyViewedButton.colors = selectedColorBlock;
            SetTextColor(Color.black);
        }

        private void SetHoverExit()
        {
            _recentlyViewedButton.colors = normalColorBlock;
            SetTextColor(Color.white);
        }

        private void SetTextColor(Color color)
        {
            _recentlyViewedText.color = color;
        }
    }
}