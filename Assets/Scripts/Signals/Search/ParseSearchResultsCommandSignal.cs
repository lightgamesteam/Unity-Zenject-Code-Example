
namespace TDL.Signals
{
    public class ParseSearchResultsCommandSignal : ISignal
    {
        public string Response { get; private set; }

        public ParseSearchResultsCommandSignal(string response)
        {
            Response = response;
        }
    }
}