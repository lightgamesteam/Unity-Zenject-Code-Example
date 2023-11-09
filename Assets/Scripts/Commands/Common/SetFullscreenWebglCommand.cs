using System.Runtime.InteropServices;
using UnityEngine;

namespace Commands.Common
{
    public class SetFullscreenWebglCommand : ICommand
    {
        [DllImport("__Internal", EntryPoint = @"SetFullScreenMode")]
        public static extern void SetFullScreenMode();
        
        public void Execute()
        {
            Debug.Log("Set FS");
            SetFullScreen();
        }

        private void SetFullScreen()
        {
            SetFullScreenMode();
        }
    }
}