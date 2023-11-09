using System.Collections;
using TDL.Models;
using TDL.Signals;
using UnityEngine.Networking;
using Zenject;

namespace TDL.Commands
{
    public class CheckInternetConnectionCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly PopupModel _popupModel;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;
        
        private const string pingServer = "https://www.3dl.no/";
        private const int internetResponseOk = 200;
        
        public void Execute(ISignal signal)
        {
            var parameter = (CheckInternetConnectionCommandSignal) signal;
            _popupModel.InternetConnectionRetrySource = parameter.SignalSource;
            
            _asyncProcessor.StartCoroutine(CheckInternetConnection());
        }

        private IEnumerator CheckInternetConnection()
        {

            bool isAvailable;
            using (var request = UnityWebRequest.Head(pingServer))
            {
                yield return request.SendWebRequest();
                isAvailable = !request.isNetworkError && !request.isHttpError && request.responseCode == internetResponseOk;
            }

            if (!isAvailable)
            {
                _signal.Fire(new ShowPopupInternetConnectionCommandSignal());
            }
        }
    }
}

