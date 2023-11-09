using UnityEngine;
using UnityEngine.UI;

public static class RectTransformExtension
{
    public static void ForceRebuildLayoutImmediate(this RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
    
    public static void MarkLayoutForRebuild(this RectTransform rect)
    {
        LayoutRebuilder.MarkLayoutForRebuild(rect);
    }
    
    public static Canvas GetMyCanvas(this RectTransform rect)
    {
        Canvas[] parentCanvases = rect.GetComponentsInParent<Canvas>();
        if (parentCanvases != null && parentCanvases.Length > 0) {
            return parentCanvases[parentCanvases.Length - 1];
        }
        
        return null;
    }
}
