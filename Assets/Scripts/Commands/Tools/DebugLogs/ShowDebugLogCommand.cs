using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class ShowDebugLogCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            #if !DISABLE_DEBUG_LOGS
                var parameter = (ShowDebugLogCommandSignal) signal;
                Debug.Log(parameter.Message);   
            #endif
        }
    }
}