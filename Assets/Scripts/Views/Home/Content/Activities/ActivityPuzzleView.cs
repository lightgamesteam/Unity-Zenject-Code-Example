using TDL.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class ActivityPuzzleView : ViewBase, IFontSizeView
    {
        private Button _clickableArea;
        private TextMeshProUGUI _title;
        
        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }
        
        public override void InitUiComponents()
        {
            _clickableArea = GetComponent<Button>();
            _title = GetComponentInChildren<TextMeshProUGUI>();
            
            DefaultFontSize = _title.fontSize;
        }

        public override void SubscribeOnListeners()
        {
            _clickableArea.onClick.AddListener(OnItemClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _clickableArea.onClick.RemoveAllListeners(); 
        }

        private void OnItemClick()
        {
            Signal.Fire(new ActivityPuzzleClickViewSignal());
        }
    
        private void ResetView()
        {
            _title.text = string.Empty;
        }

        public class Pool : MonoMemoryPool<Transform, ActivityPuzzleView>
        {
            protected override void Reinitialize(Transform viewParent, ActivityPuzzleView view)
            {
                if (view.transform.parent == null)
                {
                    view.transform.SetParent(viewParent, false);
                }

                view.ResetView();
            }
        }
    }   
}