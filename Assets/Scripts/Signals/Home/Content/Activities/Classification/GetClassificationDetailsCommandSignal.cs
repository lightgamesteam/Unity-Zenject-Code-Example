
public class GetClassificationDetailsCommandSignal : ISignal
{
    public int Id { get; private set; }

    public GetClassificationDetailsCommandSignal(int id)
    {
        Id = id;
    }
}