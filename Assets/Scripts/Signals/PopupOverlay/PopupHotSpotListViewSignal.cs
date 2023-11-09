using System;
using System.Collections.Generic;

namespace TDL.Signals
{
    public class PopupHotSpotListViewSignal : ISignal
    {
        public string PanelName { get; private set; }
        public List<(string label, int assetId, string assetType, bool hasPlusButton)> HotSpotList { get; private set; }
        public string CultureCode { get; private set; }
        public Action<int, string, bool> Callback { get; private set; }
        public PopupHotSpotListViewSignal(string panelName, List<(string label, int assetId, string assetType, bool hasPlusButton)> hotSpotList, string cultureCode, Action<int, string, bool> callback)
        {
            PanelName = panelName;
            HotSpotList = hotSpotList;
            CultureCode = cultureCode;
            Callback = callback;
        }
    }
}