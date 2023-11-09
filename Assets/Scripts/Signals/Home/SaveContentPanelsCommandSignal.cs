using UnityEngine;

public class SaveContentPanelsCommandSignal : ISignal
{
    public Transform LeftMenuContent { get; private set; }
    public Transform TopicsSubtopicsContent { get; private set; }
    public Transform AssetsContent { get; private set; }
    
    public SaveContentPanelsCommandSignal(Transform leftMenuContent, Transform topicsSubtopicsContent, Transform assetsContent)
    {
        LeftMenuContent = leftMenuContent;
        TopicsSubtopicsContent = topicsSubtopicsContent;
        AssetsContent = assetsContent;
    }
}