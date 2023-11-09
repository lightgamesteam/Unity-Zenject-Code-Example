using System;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Views
{
    public class TopicItemViewMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            _signal.Subscribe<TopicItemClickViewSignal>(OnTopicItemClick);

            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<TopicItemClickViewSignal>(OnTopicItemClick);

            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        private void OnTopicItemClick(TopicItemClickViewSignal signal)
        {
            _signal.Fire(new TopicItemClickCommandSignal(signal.ParentId, signal.Id));
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

        #region Localization

        private void ChangeUiInterface()
        {
            //ChangeAssetNamesUi();
        }

    //    private void ChangeAssetNamesUi()
    //    {
    //        foreach (var topicItemView in _homeModel.ShownTopicsOnHome)
    //        {
    //            var assetModel = _contentModel.GetTopicById(topicItemView.Id);
    //			
    //            var assetLocalizedName =
    //                assetModel.Topic .LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
    //                    ? assetModel.Asset.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
    //                    : assetModel.Asset.LocalizedName[_localizationModel.FallbackCultureCode];
    //			
    //            topicItemView.SetName(assetLocalizedName);
    //        }
    //    }

        #endregion
    }
}