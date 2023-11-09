using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Selectable))]
public class SelectableTagScrollable : SelectableTag
{
    [SerializeField]
    private ScrollRect scrollRect;
    
    public void ScrollRectToSelection(RectTransform selection)
    {
        if (scrollRect == null)
        {
            scrollRect = transform.GetComponentInParent<ScrollRect>();
        }

        if(scrollRect.vertical)
            UpdateVerticalScrollPosition(selection);
    }
    
    private void UpdateVerticalScrollPosition(RectTransform selection)
    {
        float offLimitsValue = scrollRect.GetScrollToCeneter(selection);

        DOTween.To(()=> scrollRect.verticalNormalizedPosition, x=> scrollRect.verticalNormalizedPosition = x, offLimitsValue, 0.2f).SetOptions(false);
    }
}