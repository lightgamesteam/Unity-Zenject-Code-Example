public class PlayAnimationCommandSignal : ISignal
{
    public bool IsPlay { get; private set; }

    public PlayAnimationCommandSignal(bool value)
    {
        IsPlay = value;
    }
}