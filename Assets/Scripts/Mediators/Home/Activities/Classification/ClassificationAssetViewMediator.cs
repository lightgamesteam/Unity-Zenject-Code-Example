using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Views
{
    public class ClassificationAssetViewMediator : IInitializable, IDisposable
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private ClassificationDetailsModel _classificationDetailsModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _homeModel.AssetsToDownload.CollectionChanged += OnAssetsToDownloadChanged;
            _classificationDetailsModel.OnClassificationDetailsModelUpdated += OnClassificationDetailsModelUpdated;

            _signal.Subscribe<ClassificationAssetItemClickViewSignal>(OnClassificationAssetItemClick);
            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        public void Dispose()
        {
            if (_homeModel != null)
            {
                _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;
            }

            if (_classificationDetailsModel != null)
            {
                _classificationDetailsModel.OnClassificationDetailsModelUpdated -= OnClassificationDetailsModelUpdated;
            }

            _signal.Unsubscribe<ClassificationAssetItemClickViewSignal>(OnClassificationAssetItemClick);
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

        private void OnClassificationDetailsModelUpdated()
        {
            _signal.Fire(new StartClassificationCommandSignal(_classificationDetailsModel.ClassificationId));
        }

        private void OnClassificationAssetItemClick(ClassificationAssetItemClickViewSignal signal)
        {
            HideSideMenus();
            StartActivity(signal.Id);
        }

        private void StartActivity(int classificationId)
        {
            _signal.Fire(new PopupOverlaySignal(true, _localizationModel.CurrentSystemTranslations[LocalizationConstants.LoadingKey]));

            SaveSignalSource(classificationId);
            var assetIds = CreateAssetIdsToDownloadDetails(classificationId);
            
            _signal.Fire(new StartAssetDetailsCommandSignal(assetIds));
        }

        private void SaveSignalSource(int classificationId)
        {
            _contentModel.AssetDetailsSignalSource = new GetClassificationDetailsCommandSignal(classificationId);
        }

        private List<(int AssetId, int GradeId)> CreateAssetIdsToDownloadDetails(int classificationId)
        {
            var activity = _contentModel.GetClassificationById(classificationId).ActivityItem;
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
            if (IsClassificationAssetSelected() && IsDownloading())
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

        private bool IsClassificationAssetSelected()
        {
            return _contentModel.SelectedAsset != null && _contentModel.SelectedAsset.IsClassificationSelected;
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
            var foundedView = _homeModel.ShownClassificationAssetsOnHome.FirstOrDefault(asset => asset.Id == activityId);
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

        private ClassificationAssetItemView GetView(int indexOfChangedAsset)
        {
            var changedDownloadAsset = _homeModel.AssetsToDownload[indexOfChangedAsset];
            var classificationId = 0;

            foreach (var assetItem in _homeModel.MultipleAssetIdQueueToDownload)
            {
                if (assetItem.Value.Contains(changedDownloadAsset.Id))
                {
                    classificationId = assetItem.Key;
                    break;
                }
            }

            return classificationId != 0
                ? _homeModel.ShownClassificationAssetsOnHome.Find(item => item.Id == classificationId)
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
            foreach (var assetItemView in _homeModel.ShownClassificationAssetsOnHome)
            {
                var assetModel = _contentModel.GetClassificationById(assetItemView.Id);

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
        
        private void SetTooltip(ClassificationAssetItemView classificationItemView)
        {
            var tooltip = classificationItemView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(classificationItemView.Title.text);
        }

        #endregion
    }
}