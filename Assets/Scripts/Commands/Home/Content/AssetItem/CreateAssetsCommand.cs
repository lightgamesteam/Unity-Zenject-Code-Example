using System.Collections.Generic;
using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateAssetsCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly ICacheService _cacheService;
        [Inject] private readonly LocalizationModel _localizationModel;
        [Inject] private readonly ApplicationSettingsInstaller.AssetItemResources _assetItemResources;
        [Inject] private readonly AssetItemView.Pool _assetPool;
        [Inject] private readonly UserLoginModel _userLoginModel;
        [Inject] private readonly MetaDataModel _metaDataModel;

        public void Execute(ISignal signal)
        {
            var isTeacher = _userLoginModel.IsTeacher;

            var foundedAssets = GetAssetsBasedOnChosenScreen(signal);
            DisableAsset(foundedAssets, AssetTypeConstants.Type_Module, ModuleConstants.Module_Astronomy);
            
            foreach (var assetModel in foundedAssets)
            {
                var assetView = _assetPool.Spawn(_homeModel.AssetsContent);
                assetView.transform.SetParent(_homeModel.AssetsContent, false);
                assetView.transform.SetAsLastSibling();
                assetView.GradeId = assetModel.Asset.GradeId;
                assetView.AssetId = assetModel.Asset.Id;
                
                assetView.SetAssetType(GetAssetType(assetModel.Asset.Type));

                SetLocalizedName(assetModel, assetView);
                SetDownloadStatus(assetModel, assetView);
                SetPuzzle(assetModel, assetView);
                SetQuiz(assetModel, assetView);
                SetGenerateInternalLink(isTeacher, assetView);

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
                
                SaveAssetOnHomeScreen(assetView);
            }
        }

        private void SetTooltip(AssetItemView assetView)
        {
            var tooltip = assetView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(assetView.Title.text);
        }

        private List<ClientAssetModel> GetAssetsBasedOnChosenScreen(ISignal signal)
        {
            if (signal is ShowFavouritesCommandSignal)
            {
                return _contentModel.GetFavoriteAssetsAlphabetical(_localizationModel.CurrentLanguageCultureCode, _localizationModel.FallbackCultureCode);
            }

            if (signal is ShowRecentlyViewedCommandSignal)
            {
                return _contentModel.GetRecentAssets(_metaDataModel.MaxRecent);
            }

            if (signal is ShowSearchAssetsCommandSignal)
            {
                var foundedAssetIds = ((ShowSearchAssetsCommandSignal) signal).FoundedAssetIds;
                var foundedAssets = _contentModel.GetSearchAssets(foundedAssetIds);

                if (foundedAssets.Count == 0)
                {
                    _homeModel.ShowNoSearchResults = true;
                }

                return foundedAssets;
            }

            return _contentModel.GetAssetsInSelectedCategory(_localizationModel.CurrentLanguageCultureCode, _localizationModel.FallbackCultureCode);
        }

        private void SetLocalizedName(ClientAssetModel assetDetailModel, AssetItemView assetView)
        {
            var localizedName = assetDetailModel.LocalizedName;

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
                var isDownloading = _homeModel.AssetsToDownload.Any(item => item.Id == assetDetailModel.Asset.Id);
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

        private void SetGenerateInternalLink(bool isTeacher, AssetItemView assetView)
        {
            assetView.SetGenerateInternalLinkVisibility(isTeacher);
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

        private void SaveAssetOnHomeScreen(AssetItemView assetView)
        {
            _homeModel.ShownAssetsOnHome.Add(assetView);
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