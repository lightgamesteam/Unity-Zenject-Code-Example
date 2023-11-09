using TMPro;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class DropdownActivityItemView : ViewBase
    {
        private TextMeshProUGUI _title;        
        private Button _button;

        private ISignal _activityItemClickSource;

        public override void InitUiComponents()
        {
            _title = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
        }
        
        public override void SubscribeOnListeners()
        {
            _button.onClick.AddListener(OnQuizItemClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void SetActivityItemClickSource(ISignal signalSource)
        {
            _activityItemClickSource = signalSource;
        }

        private void OnQuizItemClick()
        {
            Signal.Fire(_activityItemClickSource);
        }
        
        public class Factory : PlaceholderFactory<DropdownActivityItemView>
        {
            
        }
    }
}