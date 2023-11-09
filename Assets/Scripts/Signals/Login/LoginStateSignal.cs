
namespace TDL.Signals
{
    public class LoginStateSignal : ISignal
    {
        public LoginState loginState;

        public LoginStateSignal(LoginState _loginState)
        {
            loginState = _loginState;
        }
    }

    public enum LoginState
    {
        Authenticating,
        UploadContentAssetsRequest,
        UploadRecentlyViewedAssetsRequest
    }
}