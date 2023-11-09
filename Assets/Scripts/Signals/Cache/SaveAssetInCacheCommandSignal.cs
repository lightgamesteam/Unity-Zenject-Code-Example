
namespace TDL.Signals
{
    public class SaveAssetInCacheCommandSignal : ISignal
    {
        public int Id { get; private set; }
        public byte[] DataResponse { get; private set; }

        public SaveAssetInCacheCommandSignal(int id, byte[] dataResponse)
        {
            Id = id;
            DataResponse = dataResponse;
        }
    }
}