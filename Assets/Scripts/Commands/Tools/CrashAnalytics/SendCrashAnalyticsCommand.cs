using System;
using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class SendCrashAnalyticsCommand : ICommandWithParameters
    {
        public void Execute(ISignal signal)
        {
            var parameter = (SendCrashAnalyticsCommandSignal) signal;
            Debug.LogException(new Exception(parameter.Message));
        }
    }
}