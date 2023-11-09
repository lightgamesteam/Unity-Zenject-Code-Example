
public class DownloadToggleClickViewSignal : ISignal
{
    public int Id { get; private set; }
    public bool IsToggleOn { get; private set; }

    public DownloadToggleClickViewSignal(int id, bool isToggleOn)
    {
        Id = id;
        IsToggleOn = isToggleOn;
    }
}