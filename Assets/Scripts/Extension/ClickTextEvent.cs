using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTextEvent : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text tmpText;
    
    public static Action<string, GameObject> onClick = delegate {  };

    private void OnEnable()
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TMP_Text>();
        }
    }

    public void Invoke()
    {
        onClick.Invoke(tmpText.text, gameObject);
    }
    
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                clickMousePosition = Input.GetTouch(0).position;
            }
        }
        else
        {
            clickMousePosition = Input.mousePosition;
        }
    }
    
    private Vector2 clickMousePosition;
    private float mouseClickDistance = 0.2f;
    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                if (Vector2.Distance(clickMousePosition, Input.GetTouch(0).position) <= mouseClickDistance)
                {
                    Invoke();
                }
            }
        }
        else
        {
            if (Vector2.Distance(clickMousePosition, Input.mousePosition) <= mouseClickDistance)
            {
                Invoke();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Invoke();
    }
}