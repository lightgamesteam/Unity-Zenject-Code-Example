using UnityEngine;
using UnityEngine.UI;

public class PaddingLayoutGroup : MonoBehaviour
{
    public LayoutGroup layoutGroup;

    public void SetLeftPadding(int value)
    {
        layoutGroup.padding.left = value;
    }
    
    public void SetRightPadding(int value)
    {
        layoutGroup.padding.right = value;
    }
    
    public void SetBottomPadding(int value)
    {
        layoutGroup.padding.bottom = value;
    }
    
    public void SetTopPadding(int value)
    {
        layoutGroup.padding.top = value;
    }
}
