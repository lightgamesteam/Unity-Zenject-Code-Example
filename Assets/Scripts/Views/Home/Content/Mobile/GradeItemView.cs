using TDL.Core;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class GradeItemView : ViewBase, IThumbnailView, IFontSizeView
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
            Signal.Fire(new GradeMenuItemClickViewSignal(ParentId, Id));
        }

        private void ResetView()
        {
            _title.text = string.Empty;
            _thumbnail.texture = null;
        }
    
        public class Pool : MonoMemoryPool<Transform, GradeItemView>
        {
            private Transform _viewParent;

            protected override void Reinitialize(Transform viewParent, GradeItemView view)
            {
                if (_viewParent == null)
                {
                    _viewParent = viewParent;
                }
            
                view.ResetView();
            }

            protected override void OnCreated(GradeItemView item)
            {
                if (item.transform.parent == null)
                {
                    item.transform.SetParent(_viewParent, false);
                }
            
                base.OnCreated(item);
            }
        }
    }
}