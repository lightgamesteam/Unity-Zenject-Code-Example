
public class FavouriteToggleClickViewSignal : ISignal
{
    public int GradeId { get; private set; }
    public int AssetId { get; private set; }
    public bool IsAdded { get; private set; }

    public FavouriteToggleClickViewSignal(int gradeId, int assetId, bool isAdded)
    {
        GradeId = gradeId;
        AssetId = assetId;
        IsAdded = isAdded;
    }
}