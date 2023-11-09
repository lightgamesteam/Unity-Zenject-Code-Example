
public class UserContentItemClickViewSignal : ISignal
{
    public int Id { get; private set; }

    public UserContentItemClickViewSignal(int id)
    {
        Id = id;
    }
}