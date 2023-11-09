using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDL.Views
{
    public class MyContentViewItem : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

        private bool _isSelected;

        private Button _myContentButton;
        private TextMeshProUGUI _myContentText;
    
        public override void InitUiComponents()
        {
#if !UNITY_WEBGL
            _myContentButton = GetComponent<Button>();
            _myContentText = GetComponentInChildren<TextMeshProUGUI>();

            Signal.Fire(new SaveRightMenuItemViewSignal(LocalizationConstants.MyContentKey, this));
#endif
            
#if UNITY_WEBGL
            gameObject.SetActive(false);
#endif
        }
        
        public override void SubscribeOnListeners()
        {
#if !UNITY_WEBGL
            _myContentButton.onClick.AddListener(OnMyContentViewClick);
#endif
        }

        public override void UnsubscribeFromListeners()
        {
#if !UNITY_WEBGL
            _myContentButton.onClick.RemoveAllListeners();
#endif
        }
        
        private void OnMyContentViewClick()
        {
            Signal.Fire(new MyContentClickViewSignal());
        }
    
        public void Select()
        {
#if !UNITY_WEBGL
_isSelected = true;
SetHoverEnter();     
#endif
        }

        public void Deselect()
        {
#if !UNITY_WEBGL
                 _isSelected = false;
            SetHoverExit();       
#endif
        }

        public bool IsSelected()
        {
#if !UNITY_WEBGL
        return _isSelected;    
#endif
            return false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if !UNITY_WEBGL
       if (!_isSelected)
            {
                SetHoverEnter();
            }     
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if !UNITY_WEBGL
        if (!_isSelected)
            {
                SetHoverExit();
            }    
#endif
        }
    
        private void SetHoverEnter()
        {
            _myContentButton.colors = selectedColorBlock;
            SetTextColor(Color.black);
        }

        private void SetHoverExit()
        {
            _myContentButton.colors = normalColorBlock;
            SetTextColor(Color.white);
        }

        private void SetTextColor(Color color)
        {
            _myContentText.color = color;
        }
    } 
}