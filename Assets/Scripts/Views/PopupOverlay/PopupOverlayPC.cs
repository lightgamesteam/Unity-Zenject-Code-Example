using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Views
{
 public class PopupOverlayPC : PopupOverlayBase
    {
        private GameObject _textBox;
        private TextMeshProUGUI txt;
        private LayoutElement _layoutElement;

        public override void InitUiComponents()
        {
            _background = transform.Find("Background").gameObject;
            _recordingFrame = transform.Find("X_RecordingFrame").gameObject;
            
            _textMessage = transform.Find("Panel/txt_Message").GetComponent<TextMeshProUGUI>();
            _textMessageRectTransform = _textMessage.transform.parent.GetComponent<RectTransform>();

            _okButton = transform.Find("Panel/btn_OK").GetComponent<Button>();
            _progressPanel = transform.Find("Panel/ProgressPanel").gameObject;
            _progressValue = _progressPanel.transform.Find("ProgressValue").GetComponent<TextMeshProUGUI>();
            _progressSlider = _progressPanel.transform.Find("ProgressSlider").GetComponent<Slider>();

            var cancelButton = _progressPanel.transform.Find("ButtonsPanel/CancelProgressButton");
            _cancelProgress = cancelButton.GetComponent<Button>();
            _cancelText = cancelButton.GetComponentInChildren<TextMeshProUGUI>();

            var closeButton = _progressPanel.transform.Find("ButtonsPanel/CloseProgressButton");
            _closeProgress = closeButton.GetComponent<Button>();
            _closeText = closeButton.GetComponentInChildren<TextMeshProUGUI>();

            _loadingBar = transform.Find("Panel/X_LoadingBar").gameObject;

            _uiContainerCanvasGroup = GameObject.Find("UIContainer")?.GetComponent<CanvasGroup>();
            _uiContainerMobileCanvasGroup = GameObject.Find("UIContainerMobile")?.GetComponent<CanvasGroup>();

            _defaultMessageFontSize = _textMessage.fontSize;
            _defaultElementsFontSize = _progressValue.fontSize;
        }

        public void ShowTextBox(string _text, bool canCancel = false)
        {
            _textBox = transform.Find("X_TextBox").gameObject;
            txt = transform.Get<TextMeshProUGUI>("X_TextBox/TextBox_ContentPanel/Viewport/Сontainer/Template_Text");
            _okButton = transform.Get<Button>("X_TextBox/buttons/btn_OK");
            _closeButton = transform.Get<Button>("X_TextBox/buttons/btn_Cancel");
            _layoutElement = _textBox.transform.Get<LayoutElement>("TextBox_ContentPanel");

            _closeButton.GetComponent<Button>().onClick.AddListener(OnCloseButtonClick);
            _okButton.GetComponent<Button>().onClick.AddListener(OnOkButtonClick);

            if (!canCancel)
            {
                _closeButton.gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
            _textBox.SetActive(true);
            txt.text = _text;

            txt.rectTransform.ForceUpdateRectTransforms();
            Canvas.ForceUpdateCanvases();

            _layoutElement.preferredHeight = txt.preferredHeight < 300f
                ? txt.preferredHeight + 65
                : _layoutElement.preferredHeight;

        }
    }
}