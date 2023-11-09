using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeLine : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public Image fill;
    public Image handle;
    
    public long minValue = 0;
    public long maxValue = 1;
    public long value = 0;
    public bool isAutoUpdateFill = true;
    
    [Serializable]
    public class OnValueChanged : UnityEvent<long> {}
    
    [Space]
    public OnValueChanged onValueChanged = new OnValueChanged();
    
    void Awake()
    {
        SetValue(minValue);
    }

    public void SetMinValue(long setMinValue)
    {
        minValue = setMinValue;
    }
    
    public void SetMaxValue(long setMaxValue)
    {
        if(setMaxValue > 0)
            maxValue = setMaxValue;
    }
    
    public void SetValue(long setValue)
    {
        value = setValue;
        fill.fillAmount = GetInverseLerpValue();
        
        if(handle != null)
            SetHandlePosition();
    }

    private void SetHandlePosition()
    {
        float x = Mathf.Lerp(fill.rectTransform.rect.xMin, fill.rectTransform.rect.xMax, GetInverseLerpValue());
        handle.rectTransform.localPosition = new Vector2(x, handle.rectTransform.localPosition.y);
    }
    
    public void ChangeValue(long changeValue)
    {
        onValueChanged.Invoke(changeValue);
        if(isAutoUpdateFill)
            SetValue(changeValue);
    }

    private float GetInverseLerpValue()
    {
        return Mathf.InverseLerp(minValue, maxValue, value);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        ClickOnTimeLine(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ClickOnTimeLine(eventData);
    }

    private void ClickOnTimeLine(PointerEventData eventData)
    {
        Vector2 localPoint;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(fill.rectTransform, eventData.position, null, out localPoint))
        {
            float p = Mathf.InverseLerp(fill.rectTransform.rect.xMin, fill.rectTransform.rect.xMax, localPoint.x);
            long v = (long)Mathf.Lerp(minValue, maxValue, p);
            ChangeValue(v);
        }
    }
}
