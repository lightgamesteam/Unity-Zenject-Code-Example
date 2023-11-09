using System.Collections.Generic;

namespace TDL.Server
{
    public class AccountLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Platform { get; set; }
        public string ClientVersion { get; set; }
    }

    public class ClientUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool TermAccepted { get; set; }
        public List<string> Roles { get; set; }
    }

    public class LoginResponse : ResponseBase
    {
        public string AuthorizationToken { get; set; }
        public ClientUser User { get; set; }
    }
    
    public class FeideLogin
    {
        public string AuthToken { get; set; }        
        public string Platform { get; set; }
        public string ClientVersion { get; set; }
    }
}