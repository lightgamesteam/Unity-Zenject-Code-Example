using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace TDL.Commands
{
    public class StartModuleAssetContentCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ICacheService _cacheService;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            if (ShouldStartModuleImmediately())
            {
                StartModuleImmediately((StartModuleAssetContentCommandSignal) signal);
            }
            else
            {
                StartDownloadingModule();
            }
        }

        private bool ShouldStartModuleImmediately()
        {
            return IsAssetAlreadyDownloaded(_contentModel.SelectedAsset) || IsVimeo();
        }

        private bool IsVimeo()
        {
            var url = _contentModel.SelectedAsset.Asset.VimeoUrl;
            return !string.IsNullOrEmpty(url) && url.Contains("vimeo");
        }

        private void StartModuleImmediately(StartModuleAssetContentCommandSignal signal)
        {
            ResetSelectedActivities();
            ShowPopupOverlay(true, signal.LoadingMessage);
            AddAssetToRecentViewed();
            StartModule(signal.ModuleName);
        }

        private void StartDownloadingModule()
        {
            ShowPopupOverlay(true, _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingKey], true);
            StartDownloadIfNotInProgress(_contentModel.SelectedAsset.Asset.Id);
        }

        private void AddAssetToRecentViewed()
        {
            _signal.Fire<AddRecentAssetCommandSignal>();
        }

        private void StartModule(string moduleName)
        {
           SceneManager.LoadSceneAsync(moduleName, LoadSceneMode.Additive);
        }

        private void StartDownloadIfNotInProgress(int assetId)
        {
            if (!IsAssetAlreadyDownloading(assetId))
            {
                _signal.Fire(new StartDownloadAssetCommandSignal(assetId));
            }
        }

        private bool IsAssetAlreadyDownloading(int assetId)
        {
            return _homeModel.AssetsToDownload.Any(item => item.Id == assetId);
        }

        private bool IsAssetAlreadyDownloaded(ClientAssetModel assetModel)
        {
            return _cacheService.IsAssetExistsAndDeleteOldVersion(assetModel.Asset.Id, assetModel.Asset.Version);
        }

        private void ShowPopupOverlay(bool status, string text, bool showProgress = false)
        {
            _signal.Fire(new PopupOverlaySignal(status, text, showProgress));
        }

        private void ResetSelectedActivities()
        {
            _contentModel.SelectedAsset.IsPuzzleSelected = false;
            _contentModel.SelectedAsset.IsQuizSelected = false;
        }
    }
}