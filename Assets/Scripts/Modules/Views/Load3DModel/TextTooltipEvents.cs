using TMPro;
using UnityEngine.EventSystems;

public class TextTooltipEvents : TooltipEvents
{
    public TextMeshProUGUI textMeshPro;
    
    public override string GetHint()
    {
        return textMeshPro.text;
    }

    public override void SetHint(string text)
    {
    }

    public override void SetHintAndLanguage(string text, string _language)
    {
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        TooltipPanel.Instance.SetEnableTooltip(GetHint());
        StartCoroutine(IsInside()); 
    }
}
