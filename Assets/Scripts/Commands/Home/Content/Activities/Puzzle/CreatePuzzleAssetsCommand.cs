using System.Linq;
using TDL.Models;
using TDL.Server;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreatePuzzleAssetsCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private readonly PuzzleAssetItemView.Pool _pool;
        [Inject] private UserLoginModel _userLoginModel;

        public void Execute()
        {
            var puzzleAssets = _contentModel.GetPuzzlesInCurrentCategory(_localizationModel.CurrentLanguageCultureCode, _localizationModel.FallbackCultureCode);
            var isTeacher = _userLoginModel.IsTeacher;

            foreach (var puzzleAsset in puzzleAssets)
            {
                foreach (var puzzle in puzzleAsset.Puzzle)
                {
                    var assetView = _pool.Spawn(_homeModel.AssetsContent);
                    assetView.transform.SetParent(_homeModel.AssetsContent, false);
                    assetView.transform.SetAsLastSibling();
                    assetView.AssetId = puzzleAsset.Asset.Id;
                    assetView.GradeId = puzzleAsset.Asset.GradeId;
                    assetView.SelectedItemId = puzzle.itemId;
                
                    Debug.Log($"Puzzle A:{assetView.AssetId}  G:{assetView.GradeId}");
                    
                    SetLocalizedName(puzzle, assetView);
                    SetDownloadStatus(assetView);
                    SaveAssetOnHomeScreen(assetView);
                
                    if (DeviceInfo.IsPCInterface())
                    {
                        SetTooltip(assetView);
                        SetFeedbackAvailability(isTeacher, assetView);
                    }   
                }
            }
        }
        
        private void SetLocalizedName(ActivityItem assetDetailModel, PuzzleAssetItemView assetItemView)
        {
            var localizedName = assetDetailModel.activityLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name); 

            if (localizedName.Count > 0)
            {
                assetItemView.Title.text = localizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                    ? localizedName[_localizationModel.CurrentLanguageCultureCode]
                    // ToDo remove this later, server should always have FallbackCultureCode (en-US)
                    : localizedName.ContainsKey(_localizationModel.FallbackCultureCode) 
                        ? localizedName[_localizationModel.FallbackCultureCode]
                        : localizedName.First().Value;
            }
            else
            {
                assetItemView.Title.text = localizedName.First().Value + " NO TRANSLATION";
            }
        }
        
        private void SetDownloadStatus(PuzzleAssetItemView assetItemView)
        {
            var isDownloading = _homeModel.AssetsToDownload.Any(item => item.Id == assetItemView.AssetId);
            assetItemView.ShowProgressSlider(isDownloading);
        }
        
        private void SaveAssetOnHomeScreen(PuzzleAssetItemView assetItemView)
        {
            _homeModel.ShownPuzzleAssetsOnHome.Add(assetItemView);
        }
        
        private void SetTooltip(PuzzleAssetItemView puzzleItemView)
        {
            var tooltip = puzzleItemView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(puzzleItemView.Title.text);
        }
        
        private void SetFeedbackAvailability(bool isAvailable, PuzzleAssetItemView assetItemView)
        {
            assetItemView.SetFeedbackAvailability(isAvailable);
        }
    }
}