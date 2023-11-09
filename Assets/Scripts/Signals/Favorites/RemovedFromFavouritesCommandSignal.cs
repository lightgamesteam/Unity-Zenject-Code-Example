
namespace TDL.Signals
{
    public class RemovedFromFavouritesCommandSignal : ISignal
    {
        public int GradeId { get; private set; }
        public int AssetId { get; private set; }
        public string Response { get; private set; }

        public RemovedFromFavouritesCommandSignal(int gradeId, int assetId, string response)
        {
            GradeId = gradeId;
            AssetId = assetId;
            Response = response;
        }
    }
}