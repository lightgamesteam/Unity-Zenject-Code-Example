using System;
using TDL.Constants;
using TDL.Models;
using Zenject;

namespace TDL.Views
{
    public class ActivityMultiplePuzzlesViewMediator : IInitializable, IDisposable
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Subscribe<ActivityMultiplePuzzlesItemClickViewSignal>(OnMultiplePuzzlesItemClick);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(UnsubscribeFromHomeActivated);
            _signal.Unsubscribe<ActivityMultiplePuzzlesItemClickViewSignal>(OnMultiplePuzzlesItemClick);
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
            var itemName = typeof(ActivityMultiplePuzzlesView).Name;
            if (_homeModel.ShownActivitiesOnHome.ContainsKey(itemName))
            {
                var view = (ActivityMultiplePuzzlesView) _homeModel.ShownActivitiesOnHome[itemName];
                var localizedTitle = _localizationModel.CurrentSystemTranslations[LocalizationConstants.ActivityMultiplePuzzlesKey];
                view.Title.text = localizedTitle;

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(view, localizedTitle);
                }
            }
        }
        
        private void SetTooltip(ActivityMultiplePuzzlesView view, string localizedTitle)
        {
            var tooltip = view.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(localizedTitle);
        }
        
        private void OnMultiplePuzzlesItemClick()
        {
            _signal.Fire<CreateMultiplePuzzleAssetsCommandSignal>();
        }
    }
}