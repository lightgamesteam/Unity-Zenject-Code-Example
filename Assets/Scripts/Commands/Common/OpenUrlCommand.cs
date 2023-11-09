using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class OpenUrlCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            var parameter = (OpenUrlCommandSignal) signal;
            
            Application.OpenURL(parameter.Url);
        }
    }
}