using System;
using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Views
{
    public class ActivityMultipleQuizViewMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private HomeModel _homeModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Subscribe<ActivityMultipleQuizItemClickViewSignal>(OnMultipleQuizzesItemClick);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Unsubscribe<ActivityMultipleQuizItemClickViewSignal>(OnMultipleQuizzesItemClick);
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
        
        private void ChangeUiInterface()
        {       
            var itemName = typeof(ActivityMultipleQuizView).Name;
            if (_homeModel.ShownActivitiesOnHome.ContainsKey(itemName))
            {
                var view = (ActivityMultipleQuizView) _homeModel.ShownActivitiesOnHome[itemName];
                var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityMultipleQuizzesKey];
                view.Title.text = localizedTitle;

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(view, localizedTitle);
                }
            }
        }
        
        private void SetTooltip(ActivityMultipleQuizView view, string localizedTitle)
        {
            var tooltip = view.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(localizedTitle);
        }
        
        private void OnMultipleQuizzesItemClick()
        {
            _signal.Fire<CreateMultipleQuizAssetsCommandSignal>();
        }
    }
}