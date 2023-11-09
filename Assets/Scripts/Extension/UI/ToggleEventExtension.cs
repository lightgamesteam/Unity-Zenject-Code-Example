using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/Toggle - Event Extension")] [RequireComponent(typeof(Toggle))]
public class ToggleEventExtension : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Toggle _toggle;

    [Serializable]
    public class OnValueChanged : UnityEvent<bool>
    {
        
    }
    
    public OnValueChanged onValueChanged = new OnValueChanged();
    
    public delegate void FocusChangeDelegate(bool focus);
    public event FocusChangeDelegate OnFocusChange;
    
    [Serializable]
    public class OnValueChangedInverted : UnityEvent<bool>
    {
        
    }
    
    public OnValueChangedInverted onValueChangedInverted = new OnValueChangedInverted();
    
    private void OnEnable()
    {
        if (_toggle == null)
        {
            _toggle = GetComponent<Toggle>();
        }
        
        _toggle.onValueChanged.AddListener(InvokeEvents);
        
        InvokeEvents(_toggle.isOn);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(InvokeEvents);
    }

    public void InvokeEvents(bool value)
    {
        onValueChanged.Invoke(value);
        onValueChangedInverted.Invoke(!value);
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        OnFocusChange?.Invoke(true);
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        OnFocusChange?.Invoke(false);
    }
}