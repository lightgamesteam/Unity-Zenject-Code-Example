using System;

namespace TDL.Signals
{
    public class PopupInfoViewSignal : ISignal
    {
        public string InfoText { get; private set; }
        public string OkButtonText { get; private set; }
        public string HelpButtonText { get; private set; }
        public string ToggleText { get; private set; }
        public Action<bool> OkCallback { get; private set; }
        public Action HelpCallback { get; private set; }
        public Action<PopupInfoView> PopupInfoCallback { get; private set; }
    
        public PopupInfoViewSignal(string infoText, string okButtonText, string helpButtonText, string toggleText, 
            Action<bool> okCallback, Action helpCallback, Action<PopupInfoView> popupInfoCallback = null)
        {
            InfoText = infoText;
            OkButtonText = okButtonText;
            HelpButtonText = helpButtonText;
            ToggleText = toggleText;
            OkCallback = okCallback;
            HelpCallback = helpCallback;
            PopupInfoCallback = popupInfoCallback;
        }
    }
}