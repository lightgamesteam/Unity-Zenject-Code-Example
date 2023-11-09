using Managers;
using Signals.Login;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
	public class ShowLoginScreenCommand : ICommand
	{
		[Inject] private readonly IWindowService _windowService;

		[Inject] private readonly UserLoginModel _loginModel;
		[Inject] private readonly SignalBus _signal;
    
		public void Execute()
		{
			if (_loginModel.IsLoggedAsUser && _loginModel.IsSigningOut)
			{
				_signal.Fire(new LoginAsGuestCommandSignal());
			}
			
			Debug.Log("ShowLoginScreenCommand => WindowConstants.Login");
			_windowService.ShowWindow(WindowConstants.Login);
		}
	}	
}