using System;
using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Views
{
    public class ActivitiesItemViewMediator : IInitializable, IDisposable
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _contentModel.OnSubjectChanged += OnCategoryChanged;
            _contentModel.OnTopicChanged += OnCategoryChanged;
            _contentModel.OnSubtopicChanged += OnCategoryChanged;

            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Subscribe<ActivityItemClickViewSignal>(OnActivitiesItemClick);
        }

        public void Dispose()
        {
            if (_contentModel != null)
            {
                _contentModel.OnSubjectChanged -= OnCategoryChanged;
                _contentModel.OnTopicChanged -= OnCategoryChanged;
                _contentModel.OnSubtopicChanged -= OnCategoryChanged;
            }
            
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Unsubscribe<ActivityItemClickViewSignal>(OnActivitiesItemClick);
        }

        private void SubscribeOnHomeActivated()
        {
            _localizationModel.OnLanguageChanged += ChangeUiInterface;
        }

        private void UnsubscribeFromHomeActivated()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= ChangeUiInterface;
            }
        }
        
        private void OnActivitiesItemClick()
        {
            _signal.Fire<CreateActivitiesScreenCommandSignal>();
        }

        private void ChangeUiInterface()
        {       
            var activitiesItemName = typeof(ActivityItemView).Name;
            if (_homeModel.ShownActivitiesOnHome.ContainsKey(activitiesItemName))
            {
                var view = (ActivityItemView) _homeModel.ShownActivitiesOnHome[activitiesItemName];
                var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivitiesKey];
                view.Title.text = localizedTitle;
                
                SetTooltip(view, localizedTitle);
            }
        }
        
        private void SetTooltip(ActivityItemView view, string localizedTitle)
        {
            var tooltip = view.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(localizedTitle);
        }

        private void OnCategoryChanged()
        {
            if (_contentModel.HasCategoryOnlyOwnAssets() && _contentModel.HasCategoryAnyActivity())
            {
                _signal.Fire<CreateActivitiesScreenWithHeadersCommandSignal>();
            }
            else if (_contentModel.HasCategoryAnyActivity())
            {
                _signal.Fire<CreateActivityItemCommandSignal>();
            }
        }
    }
}