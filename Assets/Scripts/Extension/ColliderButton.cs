using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ColliderButton : MonoBehaviour
{
    public bool Interactable = true;

    [Header("Events")]
    public OnEnter onEnter = new OnEnter();
    public OnExit onExit = new OnExit();
    public OnClick onClick = new OnClick();
    
    [Serializable]
    public class OnEnter : UnityEvent
    {
        
    }
    
    [Serializable]
    public class OnExit : UnityEvent
    {
        
    }
    
    [Serializable]
    public class OnClick : UnityEvent
    {
        
    }

    //--------------------------------
    private bool touchDown = false;
    private float timer = 0;
    private float waitTouch = 0.2f;
    private bool isMouseEnter = false;
    private Vector2 clickMousePosition;
    private float mouseClickDistance => DeviceInfo.IsMobile() ? 7f : 0.5f;
    
    
    void Update()
    {
        if (touchDown)
        {
            timer += Time.deltaTime;
        }
    }
    
    private void OnMouseDown()
    {
        if (!Interactable)
            return;
        
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                if(EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return;
                
                clickMousePosition = Input.GetTouch(0).position;
                timer = 0;
                touchDown = isMouseEnter = true;
            }
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            clickMousePosition = Input.mousePosition;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!Interactable)
            return;

        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                if(EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return;

                if (touchDown)
                {
                    if (timer <= waitTouch && Vector2.Distance(clickMousePosition, Input.GetTouch(0).position) <= mouseClickDistance)
                    {
                        touchDown = false;
                        Click();
                     
                    }
                    else
                    {
                        touchDown = false;
                        Exit();
                    }
                }
            }
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (Vector2.Distance(clickMousePosition, Input.mousePosition) <= mouseClickDistance)
            {
                Click();
            }
        }
    }

    public void OnMouseEnter()
    {
        if (!Interactable)
            return;

        Enter();
    }
    
    private void OnMouseExit()
    {
        if(!Interactable)
            return;
        
        Exit();
    }
    
    private void Enter()
    {
        Debug.Log("Enter");
        onEnter.Invoke();
    }

    private void Exit()
    {
        Debug.Log("Exit");

        onExit.Invoke();
    }
    
    private void Click()
    {
        Debug.Log("Click");

        onClick.Invoke();
    }
}
