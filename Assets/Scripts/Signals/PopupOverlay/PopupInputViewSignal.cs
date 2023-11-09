using System;

namespace TDL.Signals
{
    public class PopupInputViewSignal : ISignal
    {
        public string PanelName { get; private set; }
        public string CultureCode { get; private set; }
        public bool IsReadonly { get; private set; }
        
        public string DefaultInputValue { get; private set; }
        public Action<bool, string> Callback { get; private set; }
    
        public PopupInputViewSignal(string panelName, string cultureCode,  Action<bool, string> callback, bool isReadonly = false, string defaultInputValue = "")
        {
            PanelName = panelName;
            CultureCode = cultureCode;
            Callback = callback;
            IsReadonly = isReadonly;
            DefaultInputValue = defaultInputValue;
        }
    }
}