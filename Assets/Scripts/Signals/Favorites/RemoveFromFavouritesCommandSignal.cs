
public class RemoveFromFavouritesCommandSignal : ISignal
{
    public int GradeId { get; private set; }
    public int AssetId { get; private set; }

    public RemoveFromFavouritesCommandSignal(int gradeId, int assetId)
    {
        GradeId = gradeId;
        AssetId = assetId;
    }
}