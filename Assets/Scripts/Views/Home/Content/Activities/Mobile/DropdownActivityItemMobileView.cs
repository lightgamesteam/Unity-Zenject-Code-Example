using TMPro;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class DropdownActivityItemMobileView : ViewBase
    {
        private TextMeshProUGUI _title;        
        private Button _button;

        private CanvasPanel _canvasPanel;
        private ISignal _activityItemClickSource;

        public override void InitUiComponents()
        {
            _title = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
        }
        
        public override void SubscribeOnListeners()
        {
            _button.onClick.AddListener(OnActivityItemClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetCanvas(CanvasPanel canvasPanel)
        {
            _canvasPanel = canvasPanel;
        }
        
        public void SetActivityItemClickSource(ISignal signalSource)
        {
            _activityItemClickSource = signalSource;
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        private void OnActivityItemClick()
        {
            _canvasPanel.onHide.RemoveAllListeners();
            _canvasPanel.CloseCanvasPanel();
            _canvasPanel.onHide.AddListener(() => Signal.Fire(_activityItemClickSource));
        }

        public class Factory : PlaceholderFactory<DropdownActivityItemMobileView>
        {
            
        }
    }
}