using BestHTTP;

namespace Signals.Login
{
    public class HandleRequestErrorSignal : ISignal
    {
        public HTTPResponse Response { get; set; }

        public HandleRequestErrorSignal(HTTPResponse response)
        {
            Response = response;
        }
    }
}