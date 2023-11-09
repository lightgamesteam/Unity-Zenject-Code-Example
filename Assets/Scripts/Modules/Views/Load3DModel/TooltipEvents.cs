using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TooltipEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string key;
    [SerializeField] private bool _toggleToSpeechInvertedResult;
    
    private string hint = "Hint";
    private string language;

    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        if (!rectTransform)
            rectTransform = gameObject.GetComponent<RectTransform>();
    }

    protected virtual TooltipPanel GetTooltipPanel() {
        return gameObject.GetFirstInScene<TooltipPanel>();
    }

    public string GetKey()
    {
        return key;
    }

    public bool GetSpeechInvertedResult() {
        return _toggleToSpeechInvertedResult;
    }


    public virtual string GetHint()
    {
        return hint;
    }
    
    public string GetLanguage()
    {
        return language;
    }

    public virtual void SetHint(string text)
    {
        hint = text;
    }
    
    public virtual void SetHintAndLanguage(string text, string _language)
    {
        hint = text;
        language = _language;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        TooltipPanel.Instance.SetEnableTooltip(hint);
        StartCoroutine(IsInside()); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipPanel.Instance.SetDisableTooltip();
    }

    private void OnDisable()
    {
        TooltipPanel.Instance?.SetDisableTooltip();
    }

    protected virtual IEnumerator IsInside()
    {
        while (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, rectTransform.GetMyCanvas().worldCamera))
        {
            yield return new WaitForEndOfFrame();
        }

        TooltipPanel.Instance.SetSafeDisableTooltip(hint);
    }
}