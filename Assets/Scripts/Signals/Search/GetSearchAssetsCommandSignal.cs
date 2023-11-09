
namespace TDL.Signals
{
    public class GetSearchAssetsCommandSignal : ISignal
    {
        public string SearchValue { get; private set; }
        public string CultureCode { get; private set; }

        public GetSearchAssetsCommandSignal(string searchValue, string cultureCode)
        {
            SearchValue = searchValue;
            CultureCode = cultureCode;
        }
    }
}