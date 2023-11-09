using System.Collections.Generic;
using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace TDL.Commands
{
    public class StartMultipleModuleAssetContentCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private LocalizationModel _localizationModel;

        public void Execute(ISignal signal)
        {
            var activityItem = GetSelectedMultipleActivity();

            var assetIdsToDownload = new List<int>();

            foreach (var assetContent in activityItem.assetContent)
            {
                var asset = _contentModel.GetAssetById(assetContent.assetId);

                var isDownloaded = IsAssetDownloaded(asset.Asset.Id, asset.Asset.Version);
                var isDownloading = IsAssetDownloading(asset.Asset.Id);

                if (!isDownloaded && !isDownloading)
                {
                    assetIdsToDownload.Add(asset.Asset.Id);
                }
            }

            if (assetIdsToDownload.Count > 0)
            {
                StartDownloading(activityItem.itemId, assetIdsToDownload);
                ShowDownloadingPopup();
            }
            else
            {
                if (IsAnyActivityAssetDownloading(activityItem.itemId))
                {
                    ShowDownloadingPopup();
                }
                else
                {
                    var parameter = (StartMultipleModuleAssetContentCommandSignal) signal;

                    ResetSelectedActivities();
                    ShowPopupOverlay(true, parameter.LoadingMessage);
                    StartModule(parameter.ModuleName);
                }
            }
        }

        private ActivityItem GetSelectedMultipleActivity()
        {
            if (_contentModel.SelectedAsset.IsMultipleQuizSelected)
            {
                return _contentModel.SelectedAsset.Quiz[0];
            }

            if (_contentModel.SelectedAsset.IsMultiplePuzzleSelected)
            {
                return _contentModel.SelectedAsset.Puzzle[0];
            }

            if (_contentModel.SelectedAsset.IsClassificationSelected)
            {
                return _contentModel.SelectedAsset.Classification;
            }

            return null;
        }

        private void ShowDownloadingPopup()
        {
            ShowPopupOverlay(true, _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingKey], true);
        }

        private void ShowPopupOverlay(bool status, string text, bool showProgress = false)
        {
            _signal.Fire(new PopupOverlaySignal(status, text, showProgress));
        }

        private void StartModule(string moduleName)
        {
            SceneManager.LoadSceneAsync(moduleName, LoadSceneMode.Additive);
        }

        private void StartDownloading(int activityId, List<int> assetIdsToDownload)
        {
            _homeModel.MultipleAssetIdQueueToDownload.Add(activityId, assetIdsToDownload);

            foreach (var assetId in assetIdsToDownload)
            {
                _signal.Fire(new StartDownloadAssetCommandSignal(assetId));
            }
        }
        
        private bool IsAssetDownloading(int assetId)
        {
            return _homeModel.AssetsToDownload.Any(item => item.Id == assetId);
        }

        private bool IsAssetDownloaded(int id, int assetVersion)
        {
            return _cacheService.IsAssetExistsAndDeleteOldVersion(id, assetVersion);
        }

        private bool IsAnyActivityAssetDownloading(int activityId)
        {
            return _homeModel.MultipleAssetIdQueueToDownload.ContainsKey(activityId);
        }

        private void ResetSelectedActivities()
        {
            _contentModel.SelectedAsset.IsPuzzleSelected = false;
            _contentModel.SelectedAsset.IsQuizSelected = false;
        }
    }
}