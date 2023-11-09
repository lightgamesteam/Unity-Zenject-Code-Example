
namespace TDL.Signals
{
    public class CreateMetaDataCommandSignal : ISignal
    {
        public string MetaDataResponse { get; private set; }

        public CreateMetaDataCommandSignal(string metaDataResponse)
        {
            MetaDataResponse = metaDataResponse;
        }
    }
}