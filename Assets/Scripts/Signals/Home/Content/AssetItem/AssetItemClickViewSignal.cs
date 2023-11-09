public class AssetItemClickViewSignal : ISignal
{
    public int AssetId { get; private set; }
    public int GradeId { get; private set; }

    public AssetItemClickViewSignal(int assetId, int gradeId = -1)
    {
        AssetId = assetId;
        GradeId = gradeId;
    }
}