using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangePasswordItem : ViewBase, ISelectableMenuItem
{
    [Header("Normal Color")] 
    public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

    [Header("Selected Color")] 
    public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

    private bool _isSelected;

    private Button _changePasswordButton;
    private TextMeshProUGUI __changePasswordText;
    
    public override void InitUiComponents()
    {
        _changePasswordButton = GetComponent<Button>();
        __changePasswordText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public override void SubscribeOnListeners()
    {
        _changePasswordButton.onClick.AddListener(OnChangePasswordViewClick);
    }

    public override void UnsubscribeFromListeners()
    {
        _changePasswordButton.onClick.RemoveAllListeners();
    }
    
    private void OnChangePasswordViewClick()
    {
        Signal.Fire(new ChangePasswordClickViewSignal());
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
        _changePasswordButton.colors = selectedColorBlock;
        SetTextColor(Color.black);
    }

    private void SetHoverExit()
    {
        _changePasswordButton.colors = normalColorBlock;
        SetTextColor(Color.white);
    }

    private void SetTextColor(Color color)
    {
        __changePasswordText.color = color;
    }
}