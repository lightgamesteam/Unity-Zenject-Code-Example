using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformAnchors : MonoBehaviour
{
    public List<RectAnchor> anchors = new List<RectAnchor>();

    private RectTransform _rectTransform;
    
    void Awake()
    {
        GetRectTransform();
    }

    private void GetRectTransform()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetAnchorFromPresets(int i)
    {
        if(!_rectTransform)
            GetRectTransform();
        
        if (i < anchors.Count && i >= 0)
        {
            _rectTransform.anchorMin = new Vector2(anchors[i].minX, anchors[i].minY);
            _rectTransform.anchorMax = new Vector2(anchors[i].maxX, anchors[i].maxY);
        }
    }
    
    public void SetAnchorMinX(float minX)
    {
        if(!_rectTransform)
            GetRectTransform();
        
        _rectTransform.anchorMin = new Vector2(minX, _rectTransform.anchorMin.y);
    }
    
    public void SetAnchorMinY(float minY)
    {
        if(!_rectTransform)
            GetRectTransform();
        
        _rectTransform.anchorMin = new Vector2(_rectTransform.anchorMin.x, minY);
    }
    
    public void SetAnchorMaxX(float maxX)
    {
        if(!_rectTransform)
            GetRectTransform();
        
        _rectTransform.anchorMax = new Vector2(maxX, _rectTransform.anchorMax.y);
    }
    
    public void SetAnchorMaxY(float maxY)
    {
        if(!_rectTransform)
            GetRectTransform();
        
        _rectTransform.anchorMax = new Vector2(_rectTransform.anchorMax.x, maxY);
    }
}

[Serializable]
public struct RectAnchor
{
    public float minX;
    public float minY;

    public float maxX;
    public float maxY;
}
