
public class StartMultiplePuzzleCommandSignal : ISignal
{
    public int Id { get; private set; }

    public StartMultiplePuzzleCommandSignal(int id)
    {
        Id = id;
    }
}
