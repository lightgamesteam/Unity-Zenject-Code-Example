
public class SaveAddedFavouriteAssetCommandSignal : ISignal
{
    public int GradeId { get; private set; }
    public int AssetId { get; private set; }
    public string Response { get; private set; }

    public SaveAddedFavouriteAssetCommandSignal(int gradeId, int assetId, string response)
    {
        GradeId = gradeId;
        AssetId = assetId;
        Response = response;
    }
}