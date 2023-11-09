using System;
using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Views
{
    public class ActivityClassificationViewMediator : IInitializable, IDisposable
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
            _signal.Subscribe<ActivityClassificationItemClickViewSignal>(OnClassificationItemClick);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Unsubscribe<ActivityClassificationItemClickViewSignal>(OnClassificationItemClick);
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
            var itemName = typeof(ActivityClassificationView).Name;
            if (_homeModel.ShownActivitiesOnHome.ContainsKey(itemName))
            {
                var view = (ActivityClassificationView) _homeModel.ShownActivitiesOnHome[itemName];
                var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityClassificationsKey];
                view.Title.text = localizedTitle;

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(view, localizedTitle);
                }
            }
        }

        private void SetTooltip(ActivityClassificationView view, string localizedTitle)
        {
            var tooltip = view.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(localizedTitle);
        }
        
        private void OnClassificationItemClick()
        {
            _signal.Fire<CreateClassificationAssetsCommandSignal>();
        }
    }
}
