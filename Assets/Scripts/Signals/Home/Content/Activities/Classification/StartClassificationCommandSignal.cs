
public class StartClassificationCommandSignal : ISignal
{
    public int Id { get; private set; }

    public StartClassificationCommandSignal(int id)
    {
        Id = id;
    }
}