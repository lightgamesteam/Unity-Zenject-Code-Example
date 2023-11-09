using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Module.Common
{
    public class ModuleEntryPoint : MonoBehaviour
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private readonly IWindowService _windowService;
        [Inject] private readonly UserLoginModel _loginModel;

        private void Awake()
        {     
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(gameObject.scene.name))
            {
                _windowService.ShowWindow(WindowConstants.Module);
                _signalBus.Fire(new PopupOverlaySignal(false));
            }
        }

        public void CloseModule()
        {
            var key = _loginModel.IsLoggedAsUser ? WindowConstants.Home : WindowConstants.Login;
            _windowService.ShowWindow(key);
            SceneManager.UnloadSceneAsync(gameObject.scene.name);
        }

        public void UnloadSceneModule()
        {
            SceneManager.UnloadSceneAsync(gameObject.scene.name);
        }
    }
}