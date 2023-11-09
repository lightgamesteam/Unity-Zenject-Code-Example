
namespace TDL.Signals
{
    public class SaveAvailableLanguagesCommandSignal : ISignal
    {
        public string LanguagesResponse { get; private set; }
    
        public SaveAvailableLanguagesCommandSignal(string languagesResponse)
        {
            LanguagesResponse = languagesResponse;
        }
    }
}