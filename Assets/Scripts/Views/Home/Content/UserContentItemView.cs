using TDL.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class UserContentItemView : ViewBase, IThumbnailView, IFontSizeView
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        
        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }
        
        [SerializeField] private Image _thumbnail;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _clickableArea;

        [Header("Icons panel")] 
        [SerializeField] private Image _assetType;

        [SerializeField] private Button _deleteButton;

        public override void InitUiComponents()
        {
            DefaultFontSize = _title.fontSize;
        }

        public override void SubscribeOnListeners()
        {
            _clickableArea.onClick.AddListener(OnItemClick);
            _deleteButton.onClick.AddListener(OnItemDelete);
        }

        public override void UnsubscribeFromListeners()
        {
            _clickableArea.onClick.RemoveAllListeners();
            _deleteButton.onClick.RemoveAllListeners();
        }

        public void SetThumbnail(Texture thumbnail)
        {
            _thumbnail.sprite = Sprite.Create(thumbnail as Texture2D, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(.5f, .5f));
        }

        public void SetAssetType(Sprite type)
        {
            _assetType.sprite = type;
        }
    
        private void OnItemClick()
        {
            Signal.Fire(new UserContentItemClickViewSignal(Id));
        }
    
        private void OnItemDelete()
        {
            Signal.Fire(new UserContentItemDeleteClickViewSignal(this));
        }
    
        private void ResetView()
        {
            _title.text = string.Empty;
            _thumbnail.sprite = null;
        }

        public class Pool : MonoMemoryPool<Transform, UserContentItemView>
        {
            protected override void Reinitialize(Transform viewParent, UserContentItemView view)
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