using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Toggle))]
public class ToggleTooltipEvents : TooltipEvents
{
    [SerializeField] private string trueKey;
    [SerializeField] private string falseKey;
    
    private string trueHint = "Hint";
    private string falseHint = "Hint";

    private Toggle _toggle;

    protected override void Awake()
    {
        base.Awake();
        
        if (!_toggle)
            _toggle = gameObject.GetComponent<Toggle>();
    }

    public string GetTrueKey()
    {
        return trueKey;
    }
    
    public string GetFalseKey()
    {
        return falseKey;
    }

    public void SetTrueHint(string text)
    {
        trueHint = text;
    }
    
    public void SetFalseHint(string text)
    {
        falseHint = text;
    }

    public override string GetHint()
    {
        return _toggle.isOn ? trueHint : falseHint;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        TooltipPanel.Instance.SetEnableTooltip(GetHint());
        StartCoroutine(IsInside()); 
    }

    protected override IEnumerator IsInside()
    {
        bool curState = _toggle.isOn;
        
        while (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, rectTransform.GetMyCanvas().worldCamera))
        {
            yield return new WaitForEndOfFrame();
            if (curState != _toggle.isOn)
            {
                TooltipPanel.Instance.SetEnableTooltip(GetHint());
                curState = _toggle.isOn;
            }
        }

        TooltipPanel.Instance.SetSafeDisableTooltip(GetHint());
    }
}