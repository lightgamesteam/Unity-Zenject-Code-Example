using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Views
{
    public class PopupOverlayMobile : PopupOverlayBase
    {
        private GameObject _overlayAndMessageBoxPanel;

        public CanvasPanel _canvasPanel;

        public override void InitUiComponents()
        {
            _overlayAndMessageBoxPanel = transform.Find("Background").gameObject;
        
            _textMessage = transform.Find("Background/Panel/txt_Message").GetComponent<TextMeshProUGUI>();

            _okButton = transform.Find("Background/Panel/btn_OK").GetComponent<Button>();
            _progressPanel = transform.Find("Background/Panel/ProgressPanel").gameObject;
            _progressValue = _progressPanel.transform.Find("ProgressValue").GetComponent<TextMeshProUGUI>();
            _progressSlider = _progressPanel.transform.Find("ProgressSlider").GetComponent<Slider>();
            _cancelProgress = _progressPanel.transform.Find("ButtonsPanel/CancelProgressButton").GetComponent<Button>();
            _closeProgress = _progressPanel.transform.Find("ButtonsPanel/CloseProgressButton").GetComponent<Button>();
        
            _loadingBar = transform.Find("Background/Panel/X_LoadingBar").gameObject;

            _recordingFrame = transform.Find("X_RecordingFrame").gameObject;
            _background = transform.Find("Background").gameObject;
        
            _defaultMessageFontSize = _textMessage.fontSize;
            _defaultElementsFontSize = _progressValue.fontSize;
        }

        public void ShowContentPanel()
        {
            _overlayAndMessageBoxPanel.SetActive(false);
            gameObject.SetActive(true);
            _canvasPanel.gameObject.SetActive(true);
        }
    }   
}