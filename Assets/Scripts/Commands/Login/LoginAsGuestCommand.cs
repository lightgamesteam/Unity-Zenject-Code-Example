using Managers;
using Signals.Login;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class LoginAsGuestCommand : ICommand
    {
        [Inject] private ServerService _serverService;
        [Inject] private ApplicationSettingsInstaller.ServerSettings _serverSettings;

        public void Execute()
        {
            var log = _serverSettings.GuestLogin;
            var pass = _serverSettings.GuestPassword;
            _serverService.GetLoginGuest(log, pass);
        }
    }
}