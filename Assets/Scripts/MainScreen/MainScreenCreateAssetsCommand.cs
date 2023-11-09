using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Signals;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;
using static TDL.Models.HomeModel;

namespace TDL.Commands
{
    public class MainScreenCreateAssetsCommand
    {
        [Inject] private MainScreenModel _screenModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly ICacheService _cacheService;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private readonly ApplicationSettingsInstaller.AssetItemResources _assetItemResources;
        [Inject] private readonly WebAssetItemView.Pool _assetPool;
        [Inject] private readonly UserLoginModel _userLoginModel;
        [Inject] private readonly MetaDataModel _metaDataModel;

        public void Execute()
        {
            var isTeacher = _userLoginModel.IsTeacher;

            var foundedAssets = GetAssets();
            DisableAsset(foundedAssets, AssetTypeConstants.Type_Module, ModuleConstants.Module_Astronomy);

            var numTotal = _assetPool.NumActive;

            for (var i = 0; i < numTotal; i++)
            {
                _assetPool.Despawn(_screenModel.PreviewAssets[i]);
            }

            foreach (var assetModel in foundedAssets)
            {
                var assetView = _assetPool.Spawn(_screenModel.AssetsContent);

                if(!_screenModel.PreviewAssets.Contains(assetView))
                    _screenModel.PreviewAssets.Add(assetView);
                
                assetView.transform.SetParent(_screenModel.AssetsContent, false);
                assetView.transform.SetAsLastSibling();
                assetView.GradeId = assetModel.Asset.GradeId;
                assetView.AssetId = assetModel.Asset.Id;
                assetView.SetAssetType(GetAssetType(assetModel.Asset.Type));

                SetLocalizedName(assetModel, assetView);
                SetDownloadStatus(assetModel, assetView);
                SetPuzzle(assetModel, assetView);
                SetQuiz(assetModel, assetView);

                SetFavoriteStatus(assetView);

                var hasDescription = _contentModel.HasDescription(assetView.AssetId, _localizationModel.CurrentLanguageCultureCode);
                SetStudentNotesAvailability(!isTeacher, assetView);

                if (DeviceInfo.IsPCInterface())
                {
                    SetDescriptionVisibility(hasDescription, assetView);
                    SetFeedbackAvailability(isTeacher, assetView);
                    SetTooltip(assetView);
                }
                else
                {
                    SetDescriptionVisibility(false, assetView);
                    SetMoreMenuStatus(assetModel, assetView);
                }
            }
            
            _signal.Fire(new CreateThumbnailsForAllPreviewSignal());
        }

        private void SetTooltip(AssetItemView assetView)
        {
            var tooltip = assetView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(assetView.Title.text);
        }

        private List<ClientAssetModel> GetAssets()
        {
            return _contentModel.GetAllAvailableAssets(12);
        }

        private void SetLocalizedName(ClientAssetModel assetDetailModel, AssetItemView assetView)
        {
            var localizedName = assetDetailModel.LocalizedName;

            if (_localizationModel.CurrentLanguageCultureCode == null)
            {
                assetView.Title.text = localizedName[_localizationModel.FallbackCultureCode];
                return;
            }

            assetView.Title.text = localizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                ? localizedName[_localizationModel.CurrentLanguageCultureCode]
                : localizedName[_localizationModel.FallbackCultureCode];
        }

        private void SetDownloadStatus(ClientAssetModel assetDetailModel, AssetItemView assetView)
        {
            var isActiveDownload = GetAssetType(assetDetailModel.Asset.Type) != _assetItemResources.TypeModule;
            assetView.SetActiveDownload(isActiveDownload);

            if (isActiveDownload)
            {
                var isAlreadyDownloaded = IsAlreadyDownloaded(assetDetailModel);
                var isDownloading = _screenModel.AssetsToDownload.Any(item => item.Id == assetDetailModel.Asset.Id);
                assetView.SetDownloadStatus(isAlreadyDownloaded || isDownloading, true);
                assetView.ShowProgressSlider(isDownloading);
            }
        }

        private bool IsAlreadyDownloaded(ClientAssetModel assetDetailModel)
        {
            return _cacheService.IsAssetExists(assetDetailModel.Asset.Id, assetDetailModel.Asset.Version);
        }

        private void SetPuzzle(ClientAssetModel assetDetailModel, AssetItemView assetView)
        {
            var puzzleCount = assetDetailModel.Puzzle.Count;
            assetView.HasDropdownMultiplePuzzle = puzzleCount > 1;
            assetView.SetPuzzleVisibility(puzzleCount);
        }

        private void SetQuiz(ClientAssetModel assetDetailModel, AssetItemView assetView)
        {
            var quizCount = assetDetailModel.Quiz.Count;
            assetView.HasDropdownMultipleQuiz = quizCount > 1;
            assetView.SetQuizVisibility(quizCount);
        }

        private void SetFavoriteStatus(AssetItemView assetView)
        {
            var fav = _contentModel.FavoriteAssets.Find(a => a.IsEqualsIds(assetView.AssetId, assetView.GradeId));
            bool isFav = fav?.Asset != null;
            assetView.SetFavoriteStatus(isFav);
        }

        private void SetDescriptionVisibility(bool isVisible, AssetItemView assetView)
        {
            assetView.SetDescriptionVisibility(isVisible);
        }

        private void SetFeedbackAvailability(bool isAvailable, AssetItemView assetView)
        {
            assetView.SetFeedbackAvailability(isAvailable);
        }

        private void SetStudentNotesAvailability(bool isAvailable, AssetItemView assetView)
        {
            assetView.SetStudentNotesAvailability(isAvailable);
        }

        private void SetMoreMenuStatus(ClientAssetModel assetDetailModel, AssetItemView assetView)
        {
            assetView.SetMoreMenuStatus(assetDetailModel.Quiz.Count > 0 || assetDetailModel.Puzzle.Count > 0);
        }

        private Sprite GetAssetType(string assetType)
        {
            switch (assetType.ToLower())
            {
                case AssetTypeConstants.Type_3D_Model:
                    return _assetItemResources.Type3DModel;

                case AssetTypeConstants.Type_3D_Video:
                    return _assetItemResources.Type3DVideo;

                case AssetTypeConstants.Type_2D_Video:
                    return _assetItemResources.Type2DVideo;

                case AssetTypeConstants.Type_Multilayered:
                    return _assetItemResources.TypeMultilayered;

                case AssetTypeConstants.Type_360_Model:
                    return _assetItemResources.Type360Model;

                case AssetTypeConstants.Type_Rigged_Model:
                    return _assetItemResources.TypeRiggedModel;

                case AssetTypeConstants.Type_Module:
                    return _assetItemResources.TypeModule;

                default:
                    return null;
            }
        }

        private void DisableAsset(List<ClientAssetModel> assetModels, string assetType, string assetName)
        {
            if (DeviceInfo.IsChromebook())
            {
                assetModels.RemoveAll(clientAsset =>
                    clientAsset.Asset.Type.ToLower().Equals(assetType)
                    && clientAsset.Asset.Name.Equals(assetName));
            }
        }
    }
}