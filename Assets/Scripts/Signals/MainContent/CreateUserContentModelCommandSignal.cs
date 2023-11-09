
namespace TDL.Signals
{
    public class CreateUserContentModelCommandSignal : ISignal
    {
        public string UserContentResponse { get; private set; }

        public CreateUserContentModelCommandSignal(string userContentResponse)
        {
            UserContentResponse = userContentResponse;
        }
    }
}