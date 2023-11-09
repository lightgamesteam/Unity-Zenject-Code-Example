
namespace TDL.Signals
{
    public class CreateContentModelSignal : ISignal
    {
        public string ContentResponse { get; private set; }

        public CreateContentModelSignal(string contentResponse)
        {
            ContentResponse = contentResponse;
        }
    }
}