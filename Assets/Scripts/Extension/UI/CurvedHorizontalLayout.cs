using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CurvedHorizontalLayout : LayoutGroup
{
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.5f), new Keyframe(1f, 0));
    private RectTransform _rect;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        Calculate();
    }
#endif
    
    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
        Calculate();
    }

    public override void SetLayoutVertical()
    {
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Calculate();
    }

    private void FindRect()
    {
        if(_rect == null)
            _rect = GetComponent<RectTransform>();
    }

    private void OnRectTransformDimensionsChange()
    {
        Calculate();
    }

    private List<RectTransform> _childRect = new List<RectTransform>();
    private void Calculate()
    {
        m_Tracker.Clear();
        _childRect.Clear();

        FindRect();

        if (transform.childCount == 0)
            return;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform) transform.GetChild(i);

            if(child.gameObject.activeSelf)
                _childRect.Add(child);
        }
        
        float step =  _childRect.Count == 1 ? 0.5f : 1f / (_childRect.Count-1);
        
        for (int i = 0; i < _childRect.Count; i++)
        {
            if(_childRect[i] != null)
            {
                m_Tracker.Add(this, _childRect[i],
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);
                
                float p =  _childRect.Count == 1 ? step : step * i;
                float pV = curve.Evaluate(p);

                var rect = _rect.rect;
                
                Vector3 pos = new Vector3(
                    Mathf.Lerp(rect.xMin + padding.left, rect.xMax - padding.right, p),
                    Mathf.Lerp(rect.yMin + padding.bottom, rect.yMax  - padding.top, pV), 
                    0);

                _childRect[i].localPosition = pos;
            }
        }
    }
}
