
namespace TDL.Signals
{
    public class CreateUserLoginModelSignal : ISignal
    {
        public string LoginResponse { get; }
        public bool AsGuest { get; }
    
        public CreateUserLoginModelSignal(string loginResponse, bool asGuest = false)
        {
            LoginResponse = loginResponse;
            AsGuest = asGuest;
        }
    }
}