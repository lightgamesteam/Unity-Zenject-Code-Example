
namespace TDL.Signals
{
    public class DownloadAvailableResourcesCommandSignal : ISignal
    {
        public string ResourcesResponse { get; private set; }
    
        public DownloadAvailableResourcesCommandSignal(string resourcesResponse)
        {
            ResourcesResponse = resourcesResponse;
        }
    }
}