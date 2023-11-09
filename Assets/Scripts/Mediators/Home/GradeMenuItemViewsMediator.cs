using System;
using TDL.Models;
using TDL.Signals;
using Zenject;

namespace TDL.Views
{
    public class GradeMenuItemViewsMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            _contentModel.OnGradeChanged += OnGradeChanged;
            _signal.Subscribe<GradeMenuItemClickViewSignal>(OnGradeMenuItemClick);

            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnLanguageChanged);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeOnLanguageChanged);
        }

        private void OnGradeChanged()
        {
            if (_homeModel.SelectedMenuItem != null)
            {
                _homeModel.SelectedMenuItem.Deselect();
            }

            string key = $"{_contentModel.SelectedGrade.Grade.id}";

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
                _contentModel.OnGradeChanged -= OnGradeChanged;
            }

            _signal.Unsubscribe<GradeMenuItemClickViewSignal>(OnGradeMenuItemClick);

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

        private void OnGradeMenuItemClick(GradeMenuItemClickViewSignal signal)
        {
            _signal.Fire(new GradeMenuItemClickCommandSignal(signal.ParentId, signal.Id));
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