using TDL.Core;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class SubjectItemView : ViewBase, IThumbnailView, IFontSizeView
    {
        public int ParentId { get; set; }
        public int Id { get; set; }
        
        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }
        
        [SerializeField]
        private RawImage _thumbnail;

        [SerializeField]
        private TextMeshProUGUI _title;
    
        [SerializeField]
        private Button _clickableArea;
        
        public override void InitUiComponents()
        {
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
            Signal.Fire(new SubjectMenuItemClickViewSignal(ParentId, Id));
        }

        private void ResetView()
        {
            _title.text = string.Empty;
            _thumbnail.texture = null;
        }
    
        public class Pool : MonoMemoryPool<Transform, SubjectItemView>
        {
            private Transform _viewParent;

            protected override void Reinitialize(Transform viewParent, SubjectItemView view)
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