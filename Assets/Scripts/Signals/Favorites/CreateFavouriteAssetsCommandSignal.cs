
public class CreateFavouriteAssetsCommandSignal : ISignal
{
    public string FavoriteAssetsResponse { get; private set; }

    public CreateFavouriteAssetsCommandSignal(string favoriteAssetsResponse)
    {
        FavoriteAssetsResponse = favoriteAssetsResponse;
    }
}