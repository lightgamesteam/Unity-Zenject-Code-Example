using System;
using UnityEngine;

namespace TDL.Signals
{
    public class RegisterScreenCommandSignal : ISignal
    {
        public Enum ScreenName { get; private set; }
        public GameObject ScreenObject { get; private set; }

        public RegisterScreenCommandSignal(Enum screenName, GameObject screenObject)
        {
            ScreenName = screenName;
            ScreenObject = screenObject;
        }
    }
}