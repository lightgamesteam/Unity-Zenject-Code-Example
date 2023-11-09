
public class LoginClickCommandSignal : ISignal
{
    public string UserName { get; private set; }
    public string UserPassword { get; private set; }
    public bool RememberUser { get; private set; }

    public LoginClickCommandSignal(string userName, string userPassword, bool rememberUser)
    {
        UserName = userName;
        UserPassword = userPassword;
        RememberUser = rememberUser;
    }
}