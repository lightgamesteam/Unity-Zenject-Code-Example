using System;
using TDL.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
 public class PopupOverlayBase : ViewBase
    {
        private const float _popupWidthPadding = 100.0f;
        protected float _defaultMessageFontSize;
        protected float _defaultElementsFontSize;

        protected TextMeshProUGUI _textMessage;
        protected TextMeshProUGUI _cancelText;
        protected TextMeshProUGUI _closeText;
        
        protected RectTransform _textMessageRectTransform;
        protected GameObject _progressPanel;
        protected Button _okButton;
        protected Button _closeButton;
        protected TextMeshProUGUI _progressValue;
        protected Slider _progressSlider;
        protected Button _cancelProgress;
        protected Button _closeProgress;
        protected GameObject _loadingBar;
        
        protected GameObject _background;
        protected GameObject _recordingFrame;

        protected CanvasGroup _uiContainerCanvasGroup;
        protected CanvasGroup _uiContainerMobileCanvasGroup;

        public override void SubscribeOnListeners()
        {
            _cancelProgress.onClick.AddListener(OnCancelProgressClick);
            _closeProgress.onClick.AddListener(OnCloseProgressClick);
            _okButton.onClick.AddListener(OnOkButtonClick);
        }

        public void AddOkOnClickCallback(Action action)
        {
            Debug.Log("Add Ok callback");
            _okButton.onClick.AddListener(() => AddListenerInternal(_okButton, action));
        }
        
        public void AddCloseOnClickCallback(Action action)
        {
            Debug.Log("Add Close callback");
            _closeButton.onClick.AddListener(() => AddListenerInternal(_closeButton, action));
        }

        private void AddListenerInternal(Button button, Action action)
        {
            action();
            button.onClick.RemoveListener(() => AddListenerInternal(button, action));
        }

        public override void UnsubscribeFromListeners()
        {
            _cancelProgress.onClick.RemoveAllListeners();
            _closeProgress.onClick.RemoveAllListeners();
        }

        public void ShowProgressPanel(bool status)
        {
            if (_progressPanel.activeInHierarchy != status)
            {
                _progressPanel.SetActive(status);
            }

            if (_progressPanel.activeInHierarchy)
            {
                ResetProgress();
            }
        }

        public void ResetProgressPanel()
        {
            _progressPanel.SetActive(false);
            ResetProgress();
        }
        
        public void UpdateProgress(float progress)
        {
            _progressValue.text = progress.ToString("0") + " %";
            _progressSlider.value = progress;
        }

        public float GetProgressValue()
        {
            return _progressSlider.value;
        }

        public void OnCancelProgressClick()
        {
            Signal.Fire(new OnCancelProgressClickViewSignal());
        }
        
        public void OnCloseProgressClick()
        {
            Signal.Fire(new OnCloseProgressClickViewSignal());
        }
        
        public void OnOkButtonClick()
        {
            _okButton.onClick.RemoveListener(OnOkButtonClick);
            _okButton.onClick.Invoke();
            Destroy(gameObject);
        }

        public void OnCloseButtonClick()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            _closeButton.onClick.Invoke();
            Destroy(gameObject);
        }
        
        private void ResetProgress()
        {
            _progressValue.text = "0 %";
            _progressSlider.value = 0;
        }

        public void ShowPopup(bool status)
        {
            SetInteractableUiContainerGroup(!status);
            gameObject.SetActive(status);
        }

        public void ShowOkButton(bool status)
        {
            _okButton.gameObject.SetActive(status);
        }

        public void ShowCloseButton(bool status)
        {
            _closeButton.gameObject.SetActive(status);
        }
        
        public void ShowLoadingBar(bool status)
        {
            _loadingBar.SetActive(status);
        }
        
        public void SetupRecordingFrame()
        {
            InitUiComponents();
            _background.SetActive(false);
            _recordingFrame.SetActive(true);
            _okButton.transform.parent.gameObject.SetActive(false);
        }

        public void SetText(string text, float fontScale)
        {
            UpdateFontSize(_textMessage, fontScale, _defaultMessageFontSize);
            UpdateFontSize(_progressValue, fontScale, _defaultElementsFontSize);

            _textMessage.text = text;
        }

        public void SetCancelText(string text, float fontScale)
        {
            UpdateFontSize(_cancelText, fontScale, _defaultElementsFontSize);

            _cancelText.text = text;
        }
        
        public void SetOKText(string text, float fontScale)
        {
            UpdateFontSize(_closeText, fontScale, _defaultElementsFontSize);

            _closeText.text = text;
        }
        
        public void SetText(string text, TextAlignmentOptions textAlignmentOptions, float fontScale, float paragraphSpacing = 0f)
        {
            UpdateFontSize(_textMessage, fontScale, _defaultMessageFontSize);
            UpdateFontSize(_progressValue, fontScale, _defaultElementsFontSize);

            _textMessage.text = text;
            _textMessage.alignment = textAlignmentOptions;
            _textMessage.paragraphSpacing = paragraphSpacing;
            
            if(DeviceInfo.GetUI() != DeviceUI.Mobile)
                _textMessageRectTransform.SetSizeWithCurrentAnchors(0, _textMessage.preferredWidth + _popupWidthPadding);
        }
        
        private void UpdateFontSize(TextMeshProUGUI text, float fontScale, float defaultSize)
        {        
            text.fontSize = defaultSize;
            var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
            if (currentFontSize != AccessibilityConstants.FontSizeMedium150)
            {
                text.fontSize = Mathf.RoundToInt(text.fontSize / fontScale);
            }
        }

        public bool IsVisible()
        {
            return gameObject.activeInHierarchy;
        }

        private void OnDestroy()
        {
            SetInteractableUiContainerGroup(true);
        }

        private void SetInteractableUiContainerGroup(bool isInteractable) {
            if (_uiContainerCanvasGroup != null) {
                _uiContainerCanvasGroup.interactable = isInteractable;
            }
            if (_uiContainerMobileCanvasGroup != null) {
                _uiContainerMobileCanvasGroup.interactable = isInteractable;
            }
        }

        public class Factory : PlaceholderFactory<PopupOverlayBase>
        {
        }
    }

    public enum PopupOverlayType
    {
        Overlay,
        MessageBox,
        TextBox,
        LanguageBox,
        LoadingBar,
        RecordingFrame
    }   
}