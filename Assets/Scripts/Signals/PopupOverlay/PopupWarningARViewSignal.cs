using System;

namespace TDL.Signals
{
    public class PopupWarningARViewSignal : ISignal
    {
        public Action OkCallback { get; private set; }
    
        public PopupWarningARViewSignal(Action okCallback)
        {
            OkCallback = okCallback;
        }
    }
}