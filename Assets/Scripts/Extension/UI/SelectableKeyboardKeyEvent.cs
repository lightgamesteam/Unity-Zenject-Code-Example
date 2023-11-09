using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableKeyboardKeyEvent : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public List<KeyCode> keys = new List<KeyCode>();
   
    public delegate void FocusChangeDelegate(bool focus);
    public event FocusChangeDelegate OnFocusChange;
    
    [Space]
    public OnKeyDown onKeyDown = new OnKeyDown();
    public OnGetKeyUp onGetKeyUp = new OnGetKeyUp();
    public OnGetKey onGetKey = new OnGetKey();

    private Selectable mySelectable;

    private void Awake()
    {
        mySelectable = GetComponent<Selectable>();
    }

    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == mySelectable.gameObject)
            KeyInput();
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        OnFocusChange?.Invoke(true);
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        OnFocusChange?.Invoke(false);
    }

    private void KeyInput()
    {
        keys.ForEach(keyInput =>
        {
            if (Input.GetKeyDown(keyInput))
            {
                onKeyDown?.Invoke(keyInput);
            }

            if (Input.GetKeyUp(keyInput))
            {
                onGetKeyUp?.Invoke(keyInput);
            }

            if (Input.GetKey(keyInput))
            {
                onGetKey?.Invoke(keyInput);
            }
        });
    }
}

[Serializable]
public class OnKeyDown : UnityEvent<KeyCode>
{
        
}
    
[Serializable]
public class OnGetKeyUp : UnityEvent<KeyCode>
{
        
}
    
[Serializable]
public class OnGetKey : UnityEvent<KeyCode>
{
        
}