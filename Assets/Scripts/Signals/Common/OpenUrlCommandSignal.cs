
namespace TDL.Signals
{
    public class OpenUrlCommandSignal : ISignal
    {
        public string Url { get; private set; }

        public OpenUrlCommandSignal(string url)
        {
            Url = url;
        }
    }
}