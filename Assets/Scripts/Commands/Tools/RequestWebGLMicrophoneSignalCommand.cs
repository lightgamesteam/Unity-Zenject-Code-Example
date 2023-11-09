using System.Runtime.InteropServices;

namespace Commands.Tools
{
    public class RequestWebGLMicrophoneSignalCommand : ICommand
    {
        [DllImport("__Internal")]
        public static extern void RequestMicrophone();
        
        public void Execute()
        {
            RequestMicrophone();
        }
    }
}