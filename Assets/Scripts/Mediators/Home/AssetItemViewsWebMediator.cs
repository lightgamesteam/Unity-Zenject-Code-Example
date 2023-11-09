using System;
using System.Collections.Generic;
using Zenject;
using System.Collections.Specialized;
using System.Linq;
using Signals.Localization;
using TDL.Commands;
using TDL.Constants;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;

namespace TDL.Views
{
    public class AssetItemViewsWebMediator : IInitializable, IDisposable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private MainScreenModel _mainScreenModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        protected virtual void SubscribeOnListeners()
        {
            _signal.Subscribe<OnLoginActivatedViewSignal>(OnHomeEnabled);
            _signal.Subscribe<OnLoginDeactivatedViewSignal>(OnHomeDisabled);

        }

        public void Dispose()
        {
            _signal.Unsubscribe<OnLoginActivatedViewSignal>(OnHomeEnabled);
            _signal.Unsubscribe<OnLoginDeactivatedViewSignal>(OnHomeDisabled);
        }

        private void OnHomeEnabled()
        {
            _localizationModel.OnLanguageChanged += ChangeUiInterface;
        }

        private void OnHomeDisabled()
        {
            if (_localizationModel != null)
            {
                _signal.Fire(new SaveCurrentLanguageToCacheSignal());
                _localizationModel.OnLanguageChanged -= ChangeUiInterface;
            }
        }

        private void ChangeUiInterface()
        {
            ChangeAssetNamesUi();
        }

        private void ChangeAssetNamesUi()
        {
            foreach (var assetItemView in _mainScreenModel.PreviewAssets)
            {
                var assetModel = _contentModel.GetAssetById(assetItemView.AssetId);

                var assetLocalizedName =
                    assetModel.LocalizedName.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                        ? assetModel.LocalizedName[_localizationModel.CurrentLanguageCultureCode]
                        : assetModel.LocalizedName[_localizationModel.FallbackCultureCode];

                assetItemView.Title.text = assetLocalizedName;

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(assetItemView);
                }
            }
        }
        
        private void SetTooltip(AssetItemView assetItemView)
        {
            if (assetItemView == null || assetItemView.gameObject == null)
            {
                return;
            }
            var tooltip = assetItemView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(assetItemView.Title.text);
        }
    }
}