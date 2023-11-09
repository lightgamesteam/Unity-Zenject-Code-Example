using System;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class FeedbackPopupMediator : IInitializable, IDisposable, IMediator
    {
        private FeedbackPopupView _view;

        [Inject] private readonly SignalBus _signal;
        [Inject] private FeedbackPopupView.Factory _factory;
        [Inject] private FeedbackModel _feedbackModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        public void Initialize()
        {
            SubscribeOnListeners();
            CreateView();
        }
        
        private void CreateView()
        {
            _view = _factory.Create();
            _view.ShowPopup(false);
        }
        
        private void SubscribeOnListeners()
        {
            _feedbackModel.OnShowMainFeedback += ShowMainFeedback;
            _feedbackModel.OnShowSentFeedback += ShowSentFeedback;
            _signal.Subscribe<SubscribeOnFeedbackPopupViewSignal>(OnViewEnable);
            _signal.Subscribe<UnsubscribeFromFeedbackPopupViewSignal>(OnViewDisable);
        }
        
        public void OnViewEnable()
        {
            _signal.Subscribe<SendFeedbackViewSignal>(SendFeedback);
            _signal.Subscribe<CancelFeedbackViewSignal>(CancelFeedback);
            _signal.Subscribe<FeedbackSentOkClickViewSignal>(OnSentOkClick);
        }

        public void OnViewDisable()
        {
            _signal.Unsubscribe<SendFeedbackViewSignal>(SendFeedback);
            _signal.Unsubscribe<CancelFeedbackViewSignal>(CancelFeedback);
            _signal.Unsubscribe<FeedbackSentOkClickViewSignal>(OnSentOkClick);
        }

        private void ShowMainFeedback(bool status)
        {
            if (_feedbackModel.Type == FeedbackModel.FeedbackType.Home)
            {
                if (status)
                {
                    _view.ShowPopup(true);

                    _view.FeedbackTitle.text = _feedbackModel.Title;
                    _view.FeedbackPlaceholderText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackPlaceholderKey);
                    _view.FeedbackSendText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackButtonSendKey);
                    _view.FeedbackCancelText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackButtonCancelKey);

                    UpdateFontSize();

                    _view.ShowMainFeedbackPanel(status);
                }
            }
        }

        private void UpdateFontSize()
        {
            _view.FeedbackSendText.fontSize = _view.DefaultSendFontSize;
            _view.FeedbackCancelText.fontSize = _view.DefaultCancelFontSize;
            _view.FeedbackPlaceholderText.fontSize = _view.DefaultPlaceholderFontSize;
            _view.FeedbackSentOkText.fontSize = _view.DefaultSentFontSize;
            _view.FeedbackSentOkButton.fontSize = _view.DefaultSentOkButtonFontSize;
            _view.FeedbackTitle.fontSize = _view.DefaultTitleFontSize;
            _view.FeedbackMessage.textComponent.fontSize = _view.DefaultMessageFontSize;
            
            var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
            if (currentFontSize != AccessibilityConstants.FontSizeMedium150)
            {
                _view.FeedbackSendText.fontSize = Mathf.RoundToInt(_view.FeedbackSendText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
                _view.FeedbackCancelText.fontSize = Mathf.RoundToInt(_view.FeedbackCancelText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
                _view.FeedbackPlaceholderText.fontSize = Mathf.RoundToInt(_view.FeedbackPlaceholderText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
                _view.FeedbackSentOkText.fontSize = Mathf.RoundToInt(_view.FeedbackSentOkText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
                _view.FeedbackSentOkButton.fontSize = Mathf.RoundToInt(_view.FeedbackSentOkButton.fontSize / _accessibilityModel.ModulesFontSizeScaler);
                _view.FeedbackTitle.fontSize = Mathf.RoundToInt(_view.FeedbackTitle.fontSize / _accessibilityModel.ModulesFontSizeScaler);
                _view.FeedbackMessage.textComponent.fontSize = Mathf.RoundToInt(_view.FeedbackMessage.textComponent.fontSize / _accessibilityModel.ModulesFontSizeScaler);
            }
        }

        private void ShowSentFeedback(bool status)
        {
            if (_feedbackModel.Type == FeedbackModel.FeedbackType.Home)
            {
                if (status)
                {
                    _view.FeedbackSentOkText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FeedbackSentOkKey);
                }
            
                _view.ShowSendingFeedbackPanel(false);
                _view.ShowSentFeedbackPanel(status);
            }
        }

        private void SendFeedback(SendFeedbackViewSignal signal)
        {
            _view.ShowSendingFeedbackPanel(true);
            _signal.Fire(new SendFeedbackCommandSignal(_feedbackModel.AssetId, signal.FeedbackMessage));
        }

        private void CancelFeedback(CancelFeedbackViewSignal signal)
        {
            CloseFeedbackPopup();
        }
        
        private void OnSentOkClick(FeedbackSentOkClickViewSignal signal)
        {
            CloseFeedbackPopup();
        }

        private void CloseFeedbackPopup()
        {
            _signal.Fire<HideFeedbackPopupCommandSignal>();
            _view.ClearFeedbackInput();
            _view.ShowPopup(false);
        }

        public void Dispose()
        {
            if (_feedbackModel != null)
            {
                _feedbackModel.OnShowMainFeedback -= ShowMainFeedback;
                _feedbackModel.OnShowSentFeedback -= ShowSentFeedback;
            }
            
            _signal.Unsubscribe<SubscribeOnFeedbackPopupViewSignal>(OnViewEnable);
            _signal.Unsubscribe<UnsubscribeFromFeedbackPopupViewSignal>(OnViewDisable);
        }
    }
}