using System;
using System.Collections.Generic;
using Zenject;
using System.Collections.Specialized;
using System.Linq;
using TDL.Commands;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;

namespace TDL.Views
{
    public class AssetItemViewsMediator : IInitializable, IDisposable
    {
        [Inject] private readonly UserLoginModel _userLoginModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<OnHomeActivatedViewSignal>(OnHomeEnabled);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(OnHomeDisabled);

            _homeModel.AssetsToDownload.CollectionChanged += OnAssetsToDownloadChanged;

            _signal.Subscribe<AssetItemClickViewSignal>(OnAssetItemClick);
            _signal.Subscribe<PuzzleClickViewSignal>(OnPuzzleClick);
            _signal.Subscribe<QuizClickViewSignal>(OnQuizClick);
            _signal.Subscribe<OnDropdownMultipleQuizClickViewSignal>(OnDropdownMultipleQuizClick);
            _signal.Subscribe<OnDropdownMultiplePuzzleClickViewSignal>(OnDropdownMultiplePuzzleClick);
            _signal.Subscribe<DescriptionClickViewSignal>(OnDescriptionButtonClick);
            _signal.Subscribe<FavouriteToggleClickViewSignal>(OnFavoriteToggleClick);
            _signal.Subscribe<DownloadToggleClickViewSignal>(OnDownloadToggleClick);
            _signal.Subscribe<ShowFeedbackPopupFromAssetViewSignal>(OnShowFeedbackClick);
            
            if (DeviceInfo.IsPCInterface())
            {
                _signal.Subscribe<MoreDropdownClickViewSignal>(ChangeMoreDropdownUi);
            }
        }

        public void Dispose()
        {
            if (_homeModel != null)
            {
                _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;
            }

            _signal.Unsubscribe<AssetItemClickViewSignal>(OnAssetItemClick);
            _signal.Unsubscribe<PuzzleClickViewSignal>(OnPuzzleClick);
            _signal.Unsubscribe<QuizClickViewSignal>(OnQuizClick);
            _signal.Unsubscribe<OnDropdownMultipleQuizClickViewSignal>(OnDropdownMultipleQuizClick);
            _signal.Unsubscribe<OnDropdownMultiplePuzzleClickViewSignal>(OnDropdownMultiplePuzzleClick);
            _signal.Unsubscribe<DescriptionClickViewSignal>(OnDescriptionButtonClick);
            _signal.Unsubscribe<FavouriteToggleClickViewSignal>(OnFavoriteToggleClick);
            _signal.Unsubscribe<DownloadToggleClickViewSignal>(OnDownloadToggleClick);
            _signal.Unsubscribe<ShowFeedbackPopupFromAssetViewSignal>(OnShowFeedbackClick);
            
            if (DeviceInfo.IsPCInterface())
            {
                _signal.Unsubscribe<MoreDropdownClickViewSignal>(ChangeMoreDropdownUi);
            }

            _signal.Unsubscribe<OnHomeActivatedViewSignal>(OnHomeEnabled);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(OnHomeDisabled);
        }

        private void OnHomeEnabled()
        {
            _localizationModel.OnLanguageChanged += ChangeUiInterface;
        }

        private void OnHomeDisabled()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= ChangeUiInterface;
            }
        }

        private void OnAssetsToDownloadChanged(object sender, NotifyCollectionChangedEventArgs changeEvent)
        {
            switch (changeEvent.Action)
            {
                case NotifyCollectionChangedAction.Remove:

                    foreach (var removedItem in changeEvent.OldItems)
                    {
                        var downloadedAsset = removedItem as HomeModel.DownloadAsset;
                        HideProgressSliderOnAssetView(downloadedAsset);
                        StartModuleIfAssetDownloadedAndPopupProgressActive(downloadedAsset);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    UpdateProgressSlider(changeEvent.NewStartingIndex);
                    break;
            }
        }

        private void HideProgressSliderOnAssetView(HomeModel.DownloadAsset downloadIedAsset)
        {
            var foundedView = _homeModel.ShownAssetsOnHome.Find(item => item.AssetId == downloadIedAsset?.Id);

            if (foundedView != null)
            {
                foundedView.ShowProgressSlider(false);
            }
        }

        private void StartModuleIfAssetDownloadedAndPopupProgressActive(HomeModel.DownloadAsset downloadIedAsset)
        {
            if (_homeModel.IsPopupProgressVisible)
            {
                if (downloadIedAsset?.Id == _contentModel?.SelectedAsset?.Asset?.Id)
                {
                    _signal.Fire(new StartModuleAgainAfterContentDownloadedCommandSignal());
                }
            }
        }

        private void UpdateProgressSlider(int indexOfChangedAsset)
        {
            var changedObject = _homeModel.AssetsToDownload[indexOfChangedAsset];
            var foundedView = _homeModel.ShownAssetsOnHome.Find(item => item.AssetId == changedObject.Id);

            if (foundedView != null)
            {
                foundedView.UpdateProgressSlider(changedObject.Progress);
            }
        }

        private void OnAssetItemClick(AssetItemClickViewSignal signal)
        {
            HideSideMenus();

            _contentModel.AssetDetailsSignalSource = new AssetItemClickCommandSignal(signal.AssetId, signal.GradeId);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<(int AssetId, int GradeId)>  {(signal.AssetId, signal.GradeId)}));
        }

        private void OnPuzzleClick(PuzzleClickViewSignal signal)
        {
            HideSideMenus();

            _contentModel.AssetDetailsSignalSource = new StartPuzzleCommandSignal(signal.AssetId, signal.SelectedItemId);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<(int AssetId, int GradeId)>  {(signal.AssetId, signal.GradeId)}));
        }

        private void OnQuizClick(QuizClickViewSignal signal)
        {
            HideSideMenus();

            _contentModel.AssetDetailsSignalSource = new StartQuizCommandSignal(signal.AssetId, signal.SelectedItemId);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<(int AssetId, int GradeId)>  {(signal.AssetId, signal.GradeId)}));
        }

        private void OnDropdownMultipleQuizClick(OnDropdownMultipleQuizClickViewSignal signal)
        {
            DestroyDropdownActivities(signal.AssetItem);
            _signal.Fire(new CreateDropdownMultipleQuizCommandSignal(signal.AssetItem));
        }
        
        private void OnDropdownMultiplePuzzleClick(OnDropdownMultiplePuzzleClickViewSignal signal)
        {
            DestroyDropdownActivities(signal.AssetItem);
            _signal.Fire(new CreateDropdownMultiplePuzzleCommandSignal(signal.AssetItem));
        }

        private void OnDownloadToggleClick(DownloadToggleClickViewSignal signal)
        {
            if (ShouldDownloadAsset(signal))
            {
                _contentModel.AssetDetailsSignalSource = new StartDownloadAssetCommandSignal(signal.Id);
                _signal.Fire(new StartAssetDetailsCommandSignal(new List<(int AssetId, int GradeId)> {(signal.Id, -1)}));
            }
            else
            {
                _contentModel.AssetDetailsSignalSource = new CancelDownloadProgressCommandSignal(signal.Id);
                _signal.Fire(new StartAssetDetailsCommandSignal(new List<(int AssetId, int GradeId)> {(signal.Id, -1)}));
            }
        }
        
        private void OnDescriptionButtonClick(DescriptionClickViewSignal signal)
        {
            var assetId = signal.Id;
            var assetIdString = assetId.ToString();
            
            if (!_homeModel.OpenedDescriptions.ContainsKey(assetIdString))
            {
                var asset = _contentModel.GetAssetById(assetId);
            
                var title = asset.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                    ? asset.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
                    : asset.LocalizedName[_localizationModel.FallbackCultureCode];

                var desc = _contentModel.GetCurrentAssetLocalDesc(assetId);
                var descriptionUrl = desc.Single(item => item.Culture.Equals(_localizationModel.CurrentLanguageCultureCode)).DescriptionUrl;

                _signal.Fire(new GetDescriptionCommandSignal(assetIdString, string.Empty, title, _localizationModel.CurrentLanguageCultureCode, descriptionUrl, true));
            }
        }

        private void OnFavoriteToggleClick(FavouriteToggleClickViewSignal signal)
        {
            if (signal.IsAdded)
            {
                _signal.Fire(new AddToFavouritesCommandSignal(signal.GradeId, signal.AssetId));
            }
            else
            {
                _signal.Fire(new RemoveFromFavouritesCommandSignal(signal.GradeId, signal.AssetId));
            }
        }

        private bool ShouldDownloadAsset(DownloadToggleClickViewSignal signal)
        {
            var assetVersion = _contentModel.GetAssetById(signal.Id).Asset.Version; 
            return signal.IsToggleOn && !_cacheService.IsAssetExistsAndDeleteOldVersion(signal.Id, assetVersion);
        }

        private void OnShowFeedbackClick(ShowFeedbackPopupFromAssetViewSignal signal)
        {
            _contentModel.AssetDetailsSignalSource = new ShowMainFeedbackPanelCommandSignal(signal.Id, FeedbackModel.FeedbackType.Home);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<(int AssetId, int GradeId)> {(signal.Id, -1)}));
        }

        private void HideSideMenus()
        {
            _signal.Fire(new ShowLeftMenuCommandSignal(false));
            _signal.Fire(new ShowRightMenuCommandSignal(false));
        }

        #region Localization

        private void ChangeUiInterface()
        {
            ChangeAssetNamesUi();
        }

        private void ChangeDescriptionVisibility()
        {
            foreach (var assetItemView in _homeModel.ShownAssetsOnHome)
            {
                var hasDescription = _contentModel.HasDescription(assetItemView.AssetId, _localizationModel.CurrentLanguageCultureCode, false, assetItemView.GradeId);
                assetItemView.SetDescriptionVisibility(hasDescription);
            }
        }

        private void ChangeAssetNamesUi()
        {
            foreach (var assetItemView in _homeModel.ShownAssetsOnHome)
            {
                var assetModel = _contentModel.GetAssetById(assetItemView.AssetId);

                var assetLocalizedName =
                    assetModel.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                        ? assetModel.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
                        : assetModel.LocalizedName[_localizationModel.FallbackCultureCode];

                assetItemView.Title.text = assetLocalizedName;

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(assetItemView);
                }
            }
        }
        
        private void SetTooltip(AssetItemView assetItemView)
        {
            var tooltip = assetItemView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(assetItemView.Title.text);
        }

        private void ChangeMoreDropdownUi(MoreDropdownClickViewSignal signal)
        {
            var isOpened = signal.IsToggleOn;
            var assetId = signal.Id;
            var foundedView = _homeModel.ShownAssetsOnHome.Find(item => item.AssetId == assetId);

            if (isOpened)
            {
                ChangeDescriptionVisibility();
                foundedView.SetMoreFavoriteAddText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreFavoriteAddKey));
                foundedView.SetMoreFavoriteRemoveText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreFavoriteRemoveKey));
                foundedView.SetMoreDownloadText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreDownloadKey));
                foundedView.SetMoreDownloadDeleteText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreDownloadDeleteKey));
                foundedView.SetMoreDescriptionText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreDescriptionKey));
                foundedView.SetStudentNotesText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.NotesKey));
                foundedView.SetMorePuzzleText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MorePuzzleKey));
                foundedView.SetMoreQuizText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreQuizKey));
                foundedView.SetMoreFeedbackText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreFeedbackKey));
                foundedView.UpdateTextToSpeechToggles();
            }
            else
            {
                DestroyDropdownActivities(foundedView);
            }
        }

        private void DestroyDropdownActivities(AssetItemView foundedView)
        {
            _signal.Fire(new DestroyDropdownActivitiesCommandSignal(foundedView));
        }

        #endregion
    }
}