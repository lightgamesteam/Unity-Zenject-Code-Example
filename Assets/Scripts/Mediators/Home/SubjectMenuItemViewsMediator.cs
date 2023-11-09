using System;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Views
{
    public class SubjectMenuItemViewsMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            _contentModel.OnSubjectChanged += OnSubjectChanged;
            _signal.Subscribe<SubjectMenuItemClickViewSignal>(OnSubjectMenuItemClick);

            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        private void OnSubjectChanged()
        {
            if (_homeModel.SelectedMenuItem != null)
            {
                _homeModel.SelectedMenuItem.Deselect();
            }

            var key = $"{_contentModel.SelectedSubject.ParentGrade.Grade.id} {_contentModel.SelectedSubject.Subject.id}";

            if (_homeModel.SelectableMenuItems.ContainsKey(key))
            {
                _homeModel.SelectedMenuItem = _homeModel.SelectableMenuItems[key];
                _homeModel.SelectedMenuItem.Select();
            }
        }

        public void Dispose()
        {
            if (_contentModel != null)
            {
                _contentModel.OnSubjectChanged -= OnSubjectChanged;
            }

            _signal.Unsubscribe<SubjectMenuItemClickViewSignal>(OnSubjectMenuItemClick);

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

        private void OnSubjectMenuItemClick(SubjectMenuItemClickViewSignal signal)
        {
            _signal.Fire(new SubjectMenuItemClickCommandSignal(signal.ParentId, signal.Id));
        }

        #region Localization

        private void ChangeUiInterface()
        {
            //ChangeAssetNamesUi();
        }

    //    private void ChangeAssetNamesUi()
    //    {
    //        foreach (var subjectItemView in _homeModel.ShownSubjectOnHome)
    //        {
    //            var assetModel = _contentModel.GetSubjectById(subjectItemView.Id);
    //			
    //            var assetLocalizedName =
    //                assetModel.Subject.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
    //                    ? assetModel.Asset.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
    //                    : assetModel.Asset.LocalizedName[_localizationModel.FallbackCultureCode];
    //			
    //            subjectItemView.SetSubjectName(assetLocalizedName);
    //        }
    //    }

        #endregion
    }
}