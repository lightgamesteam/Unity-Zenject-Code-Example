using System;
using System.Collections.Generic;
using TDL.Models;
using TDL.Signals;
using Zenject;
using NotifyCollectionChangedEventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;

namespace TDL.Views
{
    public class QuizAssetViewMediator : IInitializable, IDisposable, IMediator
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
            
            _signal.Subscribe<QuizAssetItemClickViewSignal>(OnQuizAssetItemClick);
            _signal.Subscribe<OnHomeActivatedViewSignal>(OnViewEnable);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(OnViewDisable);
            _signal.Subscribe<ShowFeedbackPopupFromQuizViewSignal>(OnShowFeedbackClick);
        }

        public void Dispose()
        {
            if (_homeModel != null)
            {
                _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;
            }

            _signal.Unsubscribe<QuizAssetItemClickViewSignal>(OnQuizAssetItemClick);
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(OnViewEnable);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(OnViewDisable);
            _signal.Unsubscribe<ShowFeedbackPopupFromQuizViewSignal>(OnShowFeedbackClick);
        }
        
        public void OnViewEnable()
        {
            _localizationModel.OnLanguageChanged += ChangeUiInterface;
        }

        public void OnViewDisable()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= ChangeUiInterface;
            }
        }

        private void OnQuizAssetItemClick(QuizAssetItemClickViewSignal signal)
        {
            HideSideMenus();
            SaveSignalSource(signal.Id, signal.SelectedItemId);
            
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {signal.Id}));
        }
        
        private void SaveSignalSource(int assetId, int selectedItemId)
        {
            _contentModel.AssetDetailsSignalSource = new StartQuizCommandSignal(assetId, selectedItemId);
        }

        private void OnAssetsToDownloadChanged(object sender, NotifyCollectionChangedEventArgs changeEvent)
        {
            switch (changeEvent.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:

                    foreach (var removedItem in changeEvent.OldItems)
                    {
                        var downloadedAsset = removedItem as HomeModel.DownloadAsset;
                        HideProgressSliderOnAssetView(downloadedAsset);
                    }

                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    UpdateProgressSlider(changeEvent.NewStartingIndex);
                    break;
            }
        }

        private void HideProgressSliderOnAssetView(HomeModel.DownloadAsset downloadIedAsset)
        {
            var foundedView = _homeModel.ShownQuizAssetsOnHome.Find(item => item.Id == downloadIedAsset?.Id);

            if (foundedView != null)
            {
                foundedView.ShowProgressSlider(false);
            }
        }

        private void UpdateProgressSlider(int indexOfChangedAsset)
        {
            var changedObject = _homeModel.AssetsToDownload[indexOfChangedAsset];
            var foundedView = _homeModel.ShownQuizAssetsOnHome.Find(item => item.Id == changedObject.Id);

            if (foundedView != null)
            {
                foundedView.UpdateProgressSlider(changedObject.Progress);
            }
        }
        
        private void OnShowFeedbackClick(ShowFeedbackPopupFromQuizViewSignal signal)
        {
            _contentModel.AssetDetailsSignalSource = new ShowMainFeedbackPanelCommandSignal(signal.Id, FeedbackModel.FeedbackType.Home);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {signal.Id}));
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
            foreach (var assetItemView in _homeModel.ShownQuizAssetsOnHome)
            {
                var assetModel = _contentModel.GetAssetById(assetItemView.Id);

                if (assetModel != null)
                {
                    if (assetModel.LocalizedName.Count > 0)
                    {
                        var assetLocalizedName =
                            assetModel.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                                ? assetModel.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
                                : assetModel.LocalizedName[_localizationModel.FallbackCultureCode];

                        assetItemView.Title.text = assetLocalizedName;
                    }
                    else
                    {
                        assetItemView.Title.text = assetModel.Asset.Name + " NO TRANSLATION";
                    }

                    if (DeviceInfo.IsPCInterface())
                    {
                        SetTooltip(assetItemView);
                    }
                }
            }
        }
        
        private void SetTooltip(QuizAssetView quizView)
        {
            var tooltip = quizView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(quizView.Title.text);
        }

        #endregion
    }
}