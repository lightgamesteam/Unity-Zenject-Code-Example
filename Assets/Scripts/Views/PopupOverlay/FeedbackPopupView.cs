using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class FeedbackPopupView : ViewBase
    {
        public TextMeshProUGUI FeedbackSendText { get; set; }
        public TextMeshProUGUI FeedbackCancelText { get; set; }
        public TextMeshProUGUI FeedbackPlaceholderText { get; set; }
        public TextMeshProUGUI FeedbackSentOkText { get; set; }
        public TextMeshProUGUI FeedbackSentOkButton { get; set; }
        public TextMeshProUGUI FeedbackTitle { get; set; }
        public TMP_InputField FeedbackMessage { get; set; }
        
        public float DefaultSendFontSize { get; set; }
        public float DefaultCancelFontSize { get; set; }
        public float DefaultPlaceholderFontSize { get; set; }
        public float DefaultSentFontSize { get; set; }
        public float DefaultSentOkButtonFontSize { get; set; }
        public float DefaultTitleFontSize { get; set; }
        public float DefaultMessageFontSize { get; set; }


        private RectTransform _mainPanel;
        private RectTransform _sendingPanel;
        private RectTransform _sentPanel;
        private Button _sendButton;
        private Button _cancelButton;
        private Button _sentOkButton;

        private CanvasGroup _uiContainerCanvasGroup;
        private CanvasGroup _uiContainerMobileCanvasGroup;

        public override void InitUiComponents()
        {
            // main panel
            _mainPanel = transform.Get<RectTransform>("Main_Panel");
            FeedbackMessage = _mainPanel.GetComponentInChildren<TMP_InputField>();
            var bottomPanel = _mainPanel.Get<RectTransform>("Bottom_Panel");
            _sendButton = bottomPanel.Get<Button>("SendButton");
            _cancelButton = bottomPanel.Get<Button>("CancelButton");
            FeedbackTitle = _mainPanel.Get<TextMeshProUGUI>("Title_Panel");
            FeedbackSendText = _sendButton.GetComponentInChildren<TextMeshProUGUI>();
            FeedbackCancelText = _cancelButton.GetComponentInChildren<TextMeshProUGUI>();
            FeedbackPlaceholderText = FeedbackMessage.placeholder.GetComponent<TextMeshProUGUI>();
            
            // sending panel
            _sendingPanel = transform.Get<RectTransform>("Sending_Panel");
            
            // sent panel
            _sentPanel = transform.Get<RectTransform>("Sent_Panel");
            _sentOkButton = _sentPanel.GetComponentInChildren<Button>();
            FeedbackSentOkText = _sentPanel.GetComponentInChildren<TextMeshProUGUI>();
            FeedbackSentOkButton = _sentPanel.Get<TextMeshProUGUI>("OkButton/Text");

            _uiContainerCanvasGroup = GameObject.Find("UIContainer")?.GetComponent<CanvasGroup>();
            _uiContainerMobileCanvasGroup = GameObject.Find("UIContainerMobile")?.GetComponent<CanvasGroup>();

            SaveDefaultFontSizes();
        }

        public override void SubscribeOnListeners()
        {
            _sendButton.onClick.AddListener(OnSendClick);
            _cancelButton.onClick.AddListener(OnCancelClick);
            _sentOkButton.onClick.AddListener(OnSentOkClick);
            FeedbackMessage.onValueChanged.AddListener(SetSendButtonStatus);
            Signal.Fire<SubscribeOnFeedbackPopupViewSignal>();
        }

        public override void UnsubscribeFromListeners()
        {
            _sendButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            _sentOkButton.onClick.RemoveAllListeners();
            FeedbackMessage.onValueChanged.RemoveAllListeners();
            Signal.Fire<UnsubscribeFromFeedbackPopupViewSignal>();
        }
        
        private void SaveDefaultFontSizes()
        {
            DefaultSendFontSize = FeedbackSendText.fontSize;
            DefaultCancelFontSize = FeedbackCancelText.fontSize;
            DefaultPlaceholderFontSize = FeedbackPlaceholderText.fontSize;
            DefaultSentFontSize = FeedbackSentOkText.fontSize;
            DefaultSentOkButtonFontSize = FeedbackSentOkButton.fontSize;
            DefaultTitleFontSize = FeedbackTitle.fontSize;
            DefaultMessageFontSize = FeedbackMessage.textComponent.fontSize;
        }

        public void ShowPopup(bool status)
        {
            SetInteractableUiContainerGroup(!status);
            gameObject.SetActive(status);
        }

        public void ShowMainFeedbackPanel(bool status)
        {
            _mainPanel.gameObject.SetActive(status);
        }
        
        public void ShowSendingFeedbackPanel(bool status)
        {
            _sendingPanel.gameObject.SetActive(status);        
        }
        
        public void ShowSentFeedbackPanel(bool status)
        {
            _sentPanel.gameObject.SetActive(status);        
        }

        public void SetTitle(string title)
        {
            FeedbackTitle.text = title;
        }

        public void ClearFeedbackInput()
        {
            FeedbackTitle.text = string.Empty;
            FeedbackMessage.text = string.Empty;
            _sendButton.interactable = false;
        }

        private void OnSendClick()
        {
            Signal.Fire(new SendFeedbackViewSignal(FeedbackTitle.text, FeedbackMessage.text));
        }

        private void OnCancelClick()
        {
            Signal.Fire<CancelFeedbackViewSignal>();
        }
        
        private void OnSentOkClick()
        {
            Signal.Fire<FeedbackSentOkClickViewSignal>();
        }

        private void SetSendButtonStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_sendButton.interactable)
                {
                    _sendButton.interactable = false;
                }
                
                return;
            }
            
            if (string.IsNullOrEmpty(value) && _sendButton.interactable)
            {
                _sendButton.interactable = false;
            }
            else if (!string.IsNullOrEmpty(value) && !_sendButton.interactable)
            {
                _sendButton.interactable = true;
            }
        }
        
        private void SetInteractableUiContainerGroup(bool isInteractable) {
            if (_uiContainerCanvasGroup != null) {
                _uiContainerCanvasGroup.interactable = isInteractable;
            }
            if (_uiContainerMobileCanvasGroup != null) {
                _uiContainerMobileCanvasGroup.interactable = isInteractable;
            }
        }
        
        public class Factory : PlaceholderFactory<FeedbackPopupView>
        {
        }
    }
}