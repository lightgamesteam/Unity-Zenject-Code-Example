
public class ClassificationAssetItemClickViewSignal : ISignal
{
    public int Id { get; private set; }

    public ClassificationAssetItemClickViewSignal(int id)
    {
        Id = id;
    }
}