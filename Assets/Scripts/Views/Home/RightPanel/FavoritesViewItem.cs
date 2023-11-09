using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDL.Views
{
    public class FavoritesViewItem : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

        private bool _isSelected;

        private Button _favoritesButton;
        private TextMeshProUGUI _favoritesText;
    
        public override void InitUiComponents()
        {
            _favoritesButton = GetComponent<Button>();
            _favoritesText = GetComponentInChildren<TextMeshProUGUI>();
        
            Signal.Fire(new SaveRightMenuItemViewSignal(LocalizationConstants.FavouritesKey, this));
        }
    
        public override void SubscribeOnListeners()
        {
            _favoritesButton.onClick.AddListener(OnFavoritesViewClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _favoritesButton.onClick.RemoveAllListeners();
        }
    
        private void OnFavoritesViewClick()
        {
            Signal.Fire(new FavoritesClickViewSignal());
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
            _favoritesButton.colors = selectedColorBlock;
            SetTextColor(Color.black);
        }

        private void SetHoverExit()
        {
            _favoritesButton.colors = normalColorBlock;
            SetTextColor(Color.white);
        }

        private void SetTextColor(Color color)
        {
            _favoritesText.color = color;
        }
    }
}