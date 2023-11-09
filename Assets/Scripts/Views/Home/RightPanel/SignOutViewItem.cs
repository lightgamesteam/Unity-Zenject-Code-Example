using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TDL.Views
{
    public class SignOutViewItem : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

        private Button _signOutButton;
        private TextMeshProUGUI _signOutText;

        public override void InitUiComponents()
        {
            _signOutButton = GetComponent<Button>();
            _signOutText = GetComponentInChildren<TextMeshProUGUI>();
        }
    
        public override void SubscribeOnListeners()
        {
            _signOutButton.onClick.AddListener(OnSignOutViewClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _signOutButton.onClick.RemoveAllListeners();
            SetHoverExit();
        }
    
        private void OnSignOutViewClick()
        {
            Signal.Fire(new SignOutClickViewSignal());
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHoverEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHoverExit();
        }

        private void SetHoverEnter()
        {
            _signOutButton.colors = selectedColorBlock;
            SetTextColor(Color.black);
        }

        private void SetHoverExit()
        {
            _signOutButton.colors = normalColorBlock;
            SetTextColor(Color.white);
        }

        private void SetTextColor(Color color)
        {
            _signOutText.color = color;
        }
    }
}