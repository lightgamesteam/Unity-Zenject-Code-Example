using IngameDebugConsole;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugLogOpener : MonoBehaviour, IPointerClickHandler
{
    private int clickCount;
    private int maxClick = 7;
    private float clickTime = 0.5f;
    private float lastClickTime = Mathf.Infinity;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < clickTime)
        {
            clickCount++;

            if (clickCount >= maxClick)
            {
                clickCount = 0;
                
                DebugLogManager.Instance.DebugLogOpener();
            }
        }
        else
        {
            clickCount = 0;
        }
        
        
        lastClickTime = Time.time;
    }
}
