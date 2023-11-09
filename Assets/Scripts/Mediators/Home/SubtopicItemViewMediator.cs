using System;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Views
{
    public class SubtopicItemViewMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            _signal.Subscribe<SubtopicItemClickViewSignal>(OnSubtopicItemClick);

            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<SubtopicItemClickViewSignal>(OnSubtopicItemClick);

            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        private void OnSubtopicItemClick(SubtopicItemClickViewSignal signal)
        {
            _signal.Fire(new SubtopicItemClickCommandSignal(signal.ParentId, signal.Id));
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
    //        foreach (var subtopicItemView in _homeModel.ShownSubtopicsOnHome)
    //        {
    //            var assetModel = _contentModel.GetSubtopicById(subtopicItemView.Id);
    //			
    //            var assetLocalizedName =
    //                assetModel.Subtopic .LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
    //                    ? assetModel.Asset.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
    //                    : assetModel.Asset.LocalizedName[_localizationModel.FallbackCultureCode];
    //			
    //            subtopicItemView.SetName(assetLocalizedName);
    //        }
    //    }

        #endregion
    }
}