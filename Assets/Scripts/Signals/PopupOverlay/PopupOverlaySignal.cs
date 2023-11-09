using System;
using TDL.Views;

namespace TDL.Signals
{
    public class PopupOverlaySignal : ISignal
    {
        public bool Active { get; private set; }
        public string PanelName { get; private set; }
        public string Message { get; private set; }
        public string CultureCode { get; private set; }
        public string LocalizationKey { get; private set; }
        public bool ShowProgress { get; private set; }
        public PopupOverlayType Type { get; private set; }

        public Action OkCallback { get; private set; }
        public Action CloseCallback { get; private set; }
    
        public PopupOverlaySignal(bool active, string message = "", bool showProgress = false, PopupOverlayType type = PopupOverlayType.Overlay, 
            string panelName = "", string cultureCode = "", string localizationKey = "", Action okCallback = null, Action closeCallback = null)
        {
            Active = active;
            PanelName = panelName;
            Message = message;
            CultureCode = cultureCode;
            LocalizationKey = localizationKey;
            Type = type;
            ShowProgress = showProgress;
            OkCallback = okCallback;
            CloseCallback = closeCallback;
        }
    }
}