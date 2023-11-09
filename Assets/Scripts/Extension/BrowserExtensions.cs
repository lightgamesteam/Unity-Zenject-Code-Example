using System.Runtime.InteropServices;

namespace Extension
{
    public class BrowserExtensions
    {
        [DllImport("__Internal", EntryPoint = @"GetQueryParameter")]
        public static extern string GetQueryParameter(string param);
    }
}