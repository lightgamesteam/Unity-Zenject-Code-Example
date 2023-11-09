using System.Collections.Generic;

namespace TDL.Server
{
    public class ResponseBase
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, string> LocalizedError { get; set; }
    }
}