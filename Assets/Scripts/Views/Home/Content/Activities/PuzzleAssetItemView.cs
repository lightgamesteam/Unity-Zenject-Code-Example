using TDL.Core;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class PuzzleAssetItemView : ViewBase, IThumbnailView, IFontSizeView
    {
        public int AssetId { get; set; }
        public int GradeId { get; set; }
        
        public int SelectedItemId { get; set; }
        
        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }
        
        private Button _clickableArea;
        private TextMeshProUGUI _title;
        private RawImage _thumbnail;
        private Slider _progressSlider;
        
        [SerializeField] private Button _feedbackButton;

        public override void InitUiComponents()
        {
            _clickableArea = GetComponent<Button>();
            _title = GetComponentInChildren<TextMeshProUGUI>();
            _thumbnail = GetComponentInChildren<RawImage>();
            _progressSlider = GetComponentInChildren<Slider>();
            _progressSlider.gameObject.SetActive(false);
            
            DefaultFontSize = _title.fontSize;
        }

        public override void SubscribeOnListeners()
        {
            _clickableArea.onClick.AddListener(OnItemClick);

            if (DeviceInfo.IsPCInterface())
            {
                _feedbackButton.onClick.AddListener(OnFeedbackButtonClick);
            }
        }

        public override void UnsubscribeFromListeners()
        {
            _clickableArea.onClick.RemoveAllListeners();
            
            if (DeviceInfo.IsPCInterface())
            {
                _feedbackButton.onClick.RemoveAllListeners();
            }
        }
        
        public void SetThumbnail(Texture thumbnail)
        {
            _thumbnail.texture = thumbnail;
        }

        public void SetFeedbackAvailability(bool isAvailable)
        {
            _feedbackButton.gameObject.SetActive(isAvailable);
        }
        
        private void OnFeedbackButtonClick()
        {
            Signal.Fire(new ShowFeedbackPopupFromPuzzleViewSignal(AssetId));
        }

        private void OnItemClick()
        {
            Signal.Fire(new PuzzleAssetItemClickViewSignal(AssetId, GradeId, SelectedItemId));
        }
        
        public void ShowProgressSlider(bool status)
        {
            if (_progressSlider.gameObject.activeSelf != status)
            {
                _progressSlider.gameObject.SetActive(status);
            }
        }
        
        public void UpdateProgressSlider(float progress)
        {
            _progressSlider.value = progress;
        }
        
        public void ResetProgress()
        {
            _progressSlider.value = 0.0f;
        }

        private void ResetView()
        {
            _title.text = string.Empty;
            
            ResetProgress();
            ShowProgressSlider(false);
        }

        public class Pool : MonoMemoryPool<Transform, PuzzleAssetItemView>
        {
            protected override void Reinitialize(Transform viewParent, PuzzleAssetItemView itemView)
            {
                if (itemView.transform.parent == null)
                {
                    itemView.transform.SetParent(viewParent, false);
                }

                itemView.ResetView();
            }
        }
    }
}