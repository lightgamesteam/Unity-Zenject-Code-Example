using System;
using System.Collections.Generic;
using Zenject;
using System.Collections.Specialized;
using System.Linq;
using TDL.Models;
using TDL.Signals;

namespace TDL.Views
{
    public class MultipleQuizAssetViewMediator : IInitializable, IDisposable
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _homeModel.AssetsToDownload.CollectionChanged += OnAssetsToDownloadChanged;

            _signal.Subscribe<MultipleQuizAssetItemClickViewSignal>(OnMultipleQuizAssetItemClick);
            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        public void Dispose()
        {
            if (_homeModel != null)
            {
                _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;
            }

            _signal.Unsubscribe<MultipleQuizAssetItemClickViewSignal>(OnMultipleQuizAssetItemClick);
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        private void SubscribeOnLanguageChanged()
        {
            _localizationModel.OnLanguageChanged += ChangeUiInterface;
        }

        private void UnsubscribeOnLanguageChanged()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= ChangeUiInterface;
            }
        }

        private void OnMultipleQuizAssetItemClick(MultipleQuizAssetItemClickViewSignal signal)
        {
            HideSideMenus();
            StartActivity(signal.Id);
        }

        private void StartActivity(int activityId)
        {
            SaveSignalSource(activityId);
            var assetIds = CreateAssetIdsToDownloadDetails(activityId);

            _signal.Fire(new StartAssetDetailsCommandSignal(assetIds));
        }
        
        private void SaveSignalSource(int activityId)
        {
            _contentModel.AssetDetailsSignalSource = new StartMultipleQuizCommandSignal(activityId);
        }

        private List<(int AssetId, int GradeId)> CreateAssetIdsToDownloadDetails(int activityId)
        {
            var activity = _contentModel.GetMultipleQuizById(activityId).ActivityItem;
            var assetIds = new List<(int AssetId, int GradeId)>();
            foreach (var activityContent in activity.assetContent)
            {
                assetIds.Add((activityContent.assetId, _contentModel.SelectedGrade.Id));
                _contentModel.MultipleAssetDetailsIds.Add(activityContent.assetId);
            }
            
            return assetIds;
        }

        private void OnAssetsToDownloadChanged(object sender, NotifyCollectionChangedEventArgs changeEvent)
        {
            if (IsMultipleQuizAssetSelected() && IsDownloading())
            {
                switch (changeEvent.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        HandleMultipleActivityDownloadFinished(changeEvent);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        UpdateProgressSlider(changeEvent.NewStartingIndex);
                        break;
                }
            }
        }

        private bool IsMultipleQuizAssetSelected()
        {
            return _contentModel.SelectedAsset != null && _contentModel.SelectedAsset.IsMultipleQuizSelected;
        }

        private bool IsDownloading()
        {
            return _homeModel.MultipleAssetIdQueueToDownload.Count > 0;
        }

        private void HandleMultipleActivityDownloadFinished(NotifyCollectionChangedEventArgs changeEvent)
        {
            foreach (var removedItem in changeEvent.OldItems)
            {
                var downloadedAsset = removedItem as HomeModel.DownloadAsset;

                var activityId = 0;
                foreach (var assetItem in _homeModel.MultipleAssetIdQueueToDownload)
                {
                    if (assetItem.Value.Contains(downloadedAsset.Id))
                    {
                        activityId = assetItem.Key;
                        break;
                    }
                }

                if (_homeModel.MultipleAssetIdQueueToDownload.ContainsKey(activityId))
                {
                    var foundMultiple = _homeModel.MultipleAssetIdQueueToDownload[activityId];

                    if (foundMultiple.Contains(downloadedAsset.Id))
                    {
                        foundMultiple.Remove(downloadedAsset.Id);
                    }

                    if (foundMultiple.Count == 0)
                    {
                        _homeModel.MultipleAssetIdQueueToDownload.Remove(activityId);
                        HideProgressSliderOnAssetView(activityId);
                    }

                    if (_homeModel.IsPopupProgressVisible && foundMultiple.Count == 0)
                    {
                        _signal.Fire<StartMultipleModuleAgainAfterContentDownloadedCommandSignal>();
                    }
                }
            }
        }

        private void HideProgressSliderOnAssetView(int activityId)
        {
            var foundedView = _homeModel.ShownMultipleQuizAssetsOnHome.FirstOrDefault(asset => asset.Id == activityId);
            if (foundedView != null)
            {
                foundedView.ShowProgressSlider(false);
            }
        }

        private void UpdateProgressSlider(int indexOfChangedAsset)
        {
            var foundedView = GetView(indexOfChangedAsset);

            if (foundedView != null)
            {
                var foundMultipleAssetIds = _homeModel.MultipleAssetIdQueueToDownload[foundedView.Id];

                var totalSize = 0.0f;
                var downloadedSize = 0.0f;

                foreach (var asset in foundMultipleAssetIds)
                {
                    var downloadAsset = _homeModel.AssetsToDownload.FirstOrDefault(item => item.Id == asset);

                    if (downloadAsset != null)
                    {
                        totalSize += downloadAsset.ItemRequest.DownloadLength;
                        downloadedSize += downloadAsset.ItemRequest.Downloaded;
                    }
                }

                var progress = downloadedSize / totalSize * 100.0f;
                if (progress > foundedView.GetProgressValue())
                {
                    foundedView.UpdateProgressSlider(progress);
                }
            }
        }

        private MultipleQuizAssetItemView GetView(int indexOfChangedAsset)
        {
            var changedDownloadAsset = _homeModel.AssetsToDownload[indexOfChangedAsset];
            var multipleQuizId = 0;

            foreach (var assetItem in _homeModel.MultipleAssetIdQueueToDownload)
            {
                if (assetItem.Value.Contains(changedDownloadAsset.Id))
                {
                    multipleQuizId = assetItem.Key;
                    break;
                }
            }

            return multipleQuizId != 0
                ? _homeModel.ShownMultipleQuizAssetsOnHome.Find(item => item.Id == multipleQuizId)
                : null;
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

        private void ChangeAssetNamesUi()
        {
            foreach (var assetItemView in _homeModel.ShownMultipleQuizAssetsOnHome)
            {
                var assetModel = _contentModel.GetMultipleQuizById(assetItemView.Id);

                if (assetModel != null)
                {
                    if (assetModel.ActivityItem.activityLocal.Length > 0)
                    {
                        var locale = assetModel.ActivityItem.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));
                        var defaultLocale = assetModel.ActivityItem.activityLocal.FirstOrDefault(assetLocal => assetLocal.Culture.Equals(_localizationModel.FallbackCultureCode));
                        var assetLocalizedName = locale != null ? locale.Name : defaultLocale.Name;

                        assetItemView.Title.text = assetLocalizedName;
                    }
                    else
                    {
                        assetItemView.Title.text = assetModel.ActivityItem.itemName + " NO TRANSLATION";
                    }

                    if (DeviceInfo.IsPCInterface())
                    {
                        SetTooltip(assetItemView);
                    }
                }
            }
        }
        
        private void SetTooltip(MultipleQuizAssetItemView quizItemView)
        {
            var tooltip = quizItemView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(quizItemView.Title.text);
        }

        #endregion
    }
}