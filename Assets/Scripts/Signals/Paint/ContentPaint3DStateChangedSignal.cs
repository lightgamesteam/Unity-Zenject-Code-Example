public class ContentPaint3DStateChangedSignal : ISignal
{
    public bool IsOnState { get; private set; }
    public bool IsClearAll { get; private set; }

    public ContentPaint3DStateChangedSignal(bool isOnState, bool isClearAll)
    {
        IsOnState = isOnState;
        IsClearAll = isClearAll;
    }
}
