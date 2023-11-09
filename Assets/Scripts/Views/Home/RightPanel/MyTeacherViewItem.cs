using TDL.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TDL.Signals;

namespace TDL.Views
{
    public class MyTeacherViewItem : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;
    
        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;
    
        private bool _isSelected;
    
        private Button _myTeacherButton;
        private TextMeshProUGUI _myTeacherText;
        
        public override void InitUiComponents()
        {
            _myTeacherButton = GetComponent<Button>();
            _myTeacherText = GetComponentInChildren<TextMeshProUGUI>();
            
            Signal.Fire(new SaveRightMenuItemViewSignal(LocalizationConstants.MyTeacherKey, this));
        }
        
        public override void SubscribeOnListeners()
        {
            _myTeacherButton.onClick.AddListener(OnMyTeacherViewClick);
        }
    
        public override void UnsubscribeFromListeners()
        {
            _myTeacherButton.onClick.RemoveAllListeners();
        }
        
        private void OnMyTeacherViewClick()
        {
            Signal.Fire(new RightMenuMyTeacherClickViewSignal());
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
            _myTeacherButton.colors = selectedColorBlock;
            SetTextColor(Color.black);
        }
    
        private void SetHoverExit()
        {
            _myTeacherButton.colors = normalColorBlock;
            SetTextColor(Color.white);
        }
    
        private void SetTextColor(Color color)
        {
            _myTeacherText.color = color;
        }
    }

}
