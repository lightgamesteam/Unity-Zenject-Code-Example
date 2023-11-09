using UnityEngine;

public class FlipOutsideLayout : MonoBehaviour
{
    public string rootCanvasName;
    
    private RectTransform dropdownRectTransform;
    private Canvas rootCanvas;
    private bool[] isFlipLayout = new bool[2];

    protected virtual void Awake()
    {
        dropdownRectTransform = GetComponent<RectTransform>();
        rootCanvas = gameObject.Get<Canvas>(rootCanvasName);
    }

    protected virtual void OnEnable()
    {
        Reposition();
    }

    protected void Reposition()
    {
        for (var i = 0; i < 2; i++)
        {
            if(isFlipLayout[i])
            {
                RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, i, false, false);
                isFlipLayout[i] = false;
            }
        }
        
        var corners = new Vector3[4];
        dropdownRectTransform.GetWorldCorners(corners);

        var rootCanvasRectTransform = rootCanvas.transform as RectTransform;
        var rootCanvasRect = rootCanvasRectTransform.rect;
        for (var axis = 0; axis < 2; axis++)
        {
            var outside = false;
            for (var i = 0; i < 4; i++)
            {
                var corner = rootCanvasRectTransform.InverseTransformPoint(corners[i]);
                if (corner[axis] < rootCanvasRect.min[axis] || corner[axis] > rootCanvasRect.max[axis])
                {
                    outside = true;
                    break;
                }
            }

            if (outside)
            {
                RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
                isFlipLayout[axis] = true;
            }
        }
    }
}
