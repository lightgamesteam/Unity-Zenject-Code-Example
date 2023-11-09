
public class StartMultipleQuizCommandSignal : ISignal
{
    public int Id { get; private set; }

    public StartMultipleQuizCommandSignal(int id)
    {
        Id = id;
    }
}
