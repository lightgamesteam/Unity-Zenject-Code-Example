using System.Collections;
using System.Runtime.InteropServices;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class LoginTeamsIfTokenAvailableCommand : ICommand
    {
        [Inject] private readonly ServerService serverService;
        [Inject] private PopupOverlayMediator _popupOverlayMediator;
        [Inject] private SignalBus _signalBus;
        [Inject] public IExecutor Courutine { get; private set; }
        
        [DllImport("__Internal")] private static extern string GetToken();

        private void ShowPopupOverlay()
        {
            _signalBus.Fire(new LoginStateSignal(LoginState.Authenticating));
        }
        
        public async void Execute()
        {
           
            if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
            {
                Courutine.Execute(wait());
                return;
            }

            Debug.Log("Editor or not webGl");
        }

        private IEnumerator wait()
        {
            
            Debug.Log("IsInTeams == TRUE");

            yield return new WaitWhile(()=> string.IsNullOrEmpty(GetToken()));

            Debug.Log("after wait token");


            var token = GetToken();

            //default logic. Don't do anything. Just return
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                Debug.Log("Default logic. Showing some login methods.");
                yield break;
            }

            //otherwise show login window but with Authenticating popup.
            //call here GetLoginTeams.

            Debug.Log("Team token received logic. Trying to authorize using it.");
            Debug.Log(token);
            serverService.TryLoginByTeams(token);
            ShowPopupOverlay();
        }
    }
}