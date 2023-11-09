using Signals.Login;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class HandleRequestErrorCommand : ICommandWithParameters
    {
        [Inject] private SignalBus _signal;
        [Inject] private ServerService _server;
        [Inject] private UserLoginModel _loginModel;
        
        public void Execute(ISignal signal)
        {
            var data = (HandleRequestErrorSignal) signal;

            if (data.Response.StatusCode == 400)
            {
                _signal.Fire(new PopupOverlaySignal(true, "Hold on."));
                _server.RefreshTeamsLogin(() =>
                {
                    var request = data.Response.baseRequest;
                    if (request.HasHeader(ServerConstants.AuthorizationToken))
                    {
                        request.SetHeader(ServerConstants.AuthorizationToken, _loginModel.AuthorizationToken);
                    }
                    request.Send();
                });
            }
        }
    }
}