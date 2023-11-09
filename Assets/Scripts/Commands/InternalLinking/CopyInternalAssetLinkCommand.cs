using System.Runtime.InteropServices;
using TDL.Models;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CopyInternalAssetLinkCommand: ICommandWithParameters
    {
        [DllImport("__Internal")]
        private static extern string CopyToClipboard(string text);
        
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localization;

        public void Execute(ISignal signal)
        {
            var parameter =(CopyInternalAssetLinkCommandSignal) signal;
            var link = parameter.Link;
#if  UNITY_WEBGL && !UNITY_EDITOR
                            CopyToClipboard(link);
#else
            GUIUtility.systemCopyBuffer = link;
#endif
            _signal.Fire(new PopupInputViewSignal("Asset id is generated", _localization.CurrentLanguageCultureCode, null, true, link));
        }
    }
}

namespace TDL.Signals
{
    public class CopyInternalAssetLinkCommandSignal : ISignal
    {
        public string Link { get; private set; }

        public CopyInternalAssetLinkCommandSignal(string link)
        {
            Link = link;
        }
    }
}