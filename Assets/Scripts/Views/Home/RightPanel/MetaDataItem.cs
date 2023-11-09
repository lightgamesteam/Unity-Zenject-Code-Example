using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDL.Views
{
    public class MetaDataItem : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

        private bool _isSelected;

        private Button _metaDataButton;
        public TextMeshProUGUI MetaDatText { get; set; }
    
        public override void InitUiComponents()
        {
            _metaDataButton = GetComponent<Button>();
            MetaDatText = GetComponentInChildren<TextMeshProUGUI>();
            Signal.Fire(new SaveRightMenuItemViewSignal(LocalizationConstants.MetaDataKey, this));
        }
    
        public override void SubscribeOnListeners()
        {
            _metaDataButton.onClick.AddListener(OnMetaDataViewClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _metaDataButton.onClick.RemoveAllListeners();
        }
    
        private void OnMetaDataViewClick()
        {
            Signal.Fire(new MetaDataClickViewSignal());
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
            _metaDataButton.colors = selectedColorBlock;
            SetTextColor(Color.black);
        }

        private void SetHoverExit()
        {
            _metaDataButton.colors = normalColorBlock;
            SetTextColor(Color.white);
        }

        private void SetTextColor(Color color)
        {
            MetaDatText.color = color;
        }
    }
}