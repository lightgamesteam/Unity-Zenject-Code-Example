using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace TDL.Commands
{
    public class MainScreenStartModuleAssetContentCommand : ICommandWithParameters
    {
        [Inject] private MainScreenModel _mainModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ICacheService _cacheService;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            //var mainScreenSignal = signal as MainScreenStartModuleAssetContentCommandSignal;
            //var asset = _mainModel.GetAssetById(mainScreenSignal.AssetId);
            //if (IsVimeo(asset.Asset.VimeoUrl))
            //{
            //    StartModuleImmediately(mainScreenSignal);
            //}
        }

        private bool IsVimeo(string url)
        {
            return !string.IsNullOrEmpty(url) && url.Contains("vimeo");
        }


        private void StartModuleImmediately(MainScreenStartModuleAssetContentCommandSignal signal)
        {
            ShowPopupOverlay(true, signal.LoadingMessage);
            StartModule(signal.ModuleName);
        }

        private void StartModule(string moduleName)
        {
           SceneManager.LoadSceneAsync(moduleName, LoadSceneMode.Additive);
        }

        private void ShowPopupOverlay(bool status, string text, bool showProgress = false)
        {
            _signal.Fire(new PopupOverlaySignal(status, text, showProgress));
        }
    }
}