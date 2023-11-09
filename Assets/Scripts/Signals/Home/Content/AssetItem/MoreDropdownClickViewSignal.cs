
public class MoreDropdownClickViewSignal : ISignal
{
    public int Id { get; private set; }
    public bool IsToggleOn { get; private set; }

    public MoreDropdownClickViewSignal(int id, bool isToggleOn)
    {
        Id = id;
        IsToggleOn = isToggleOn;
    }
}