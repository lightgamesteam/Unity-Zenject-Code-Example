
namespace TDL.Signals
{
    public class SaveAvailableResourcesCommandSignal : ISignal
    {
        public string ResourcesResponse { get; private set; }
    
        public SaveAvailableResourcesCommandSignal(string resourcesResponse)
        {
            ResourcesResponse = resourcesResponse;
        }
    }
}