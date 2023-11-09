using TDL.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
 public class MultiplePuzzleAssetItemView : ViewBase, IThumbnailView, IFontSizeView
    {
        public int Id { get; set; }
        
        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }
        
        private Button _clickableArea;
        private TextMeshProUGUI _title;
        private RawImage _thumbnail;
        private Slider _progressSlider;

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
        }

        public override void UnsubscribeFromListeners()
        {
            _clickableArea.onClick.RemoveAllListeners();
        }
        
        public void SetThumbnail(Texture thumbnail)
        {
            _thumbnail.texture = thumbnail;
        }

        private void OnItemClick()
        {
            Signal.Fire(new MultiplePuzzleAssetItemClickViewSignal(Id));
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
        
        public float GetProgressValue()
        {
            return _progressSlider.value;
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

        public class Pool : MonoMemoryPool<Transform, MultiplePuzzleAssetItemView>
        {
            protected override void Reinitialize(Transform viewParent, MultiplePuzzleAssetItemView itemView)
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