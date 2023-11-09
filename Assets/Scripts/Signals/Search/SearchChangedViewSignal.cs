
namespace TDL.Signals
{
    public class SearchChangedViewSignal : ISignal
    {
        public string SearchValue { get; private set; }

        public SearchChangedViewSignal(string searchValue)
        {
            SearchValue = searchValue;
        }
    }
}