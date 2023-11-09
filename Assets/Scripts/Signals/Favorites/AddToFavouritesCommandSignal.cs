
public class AddToFavouritesCommandSignal : ISignal {

	public int GradeId { get; private set; }
	public int AssetId { get; private set; }

	public AddToFavouritesCommandSignal(int graderId, int assetId)
	{
		GradeId = graderId;
		AssetId = assetId;
	}
}
