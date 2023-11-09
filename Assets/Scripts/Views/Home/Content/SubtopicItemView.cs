using TDL.Core;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class SubtopicItemView : ViewBase, IThumbnailView, IFontSizeView
    {
        public int ParentId { get; set; }
        public int Id { get; set; }

        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }

        [SerializeField]
        public GameObject Video2DIcon;
        [SerializeField]
        public TextMeshProUGUI Video2DNumber;

        public void SetVideo2D(int number)
        {
            SetSubtopicItem(number, Video2DIcon, Video2DNumber);
        }
            
        [SerializeField]
        public GameObject Video3DIcon;
        [SerializeField]
        public TextMeshProUGUI Video3DNumber;
        
        public void SetVideo3D(int number)
        {
            SetSubtopicItem(number, Video3DIcon, Video3DNumber);
        }
        
        [SerializeField]
        public GameObject Model3DIcon;
        [SerializeField]
        public TextMeshProUGUI Model3DNumber;
        
        public void SetModel3D(int number)
        {
            SetSubtopicItem(number, Model3DIcon, Model3DNumber);
        }

        [SerializeField]
        public GameObject Model360Icon;
        [SerializeField]
        public TextMeshProUGUI Model360Number;
        
        public void SetModel360(int number)
        {
            SetSubtopicItem(number, Model360Icon, Model360Number);
        }
        
        [SerializeField]
        public GameObject ModelRiggedIcon;
        [SerializeField]
        public TextMeshProUGUI ModelRiggedNumber;
        
        public void SetModelRigged(int number)
        {
            SetSubtopicItem(number, ModelRiggedIcon, ModelRiggedNumber);
        }
        
        [SerializeField]
        public GameObject ModulesIcon;
        [SerializeField]
        public TextMeshProUGUI ModulesNumber;
        
        public void SetModules(int number)
        {
            SetSubtopicItem(number, ModulesIcon, ModulesNumber);
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
        
        private void SetSubtopicItem(int number, GameObject icon, TextMeshProUGUI textNumber)
        {
            icon.SetActive(number > 0);
            textNumber.gameObject.SetActive(number > 0);
            textNumber.text = number.ToString();
        }

        private void OnItemClick()
        {
            Signal.Fire(new SubtopicItemClickViewSignal(ParentId, Id));
        }

        private void ResetView()
        {
            _title.text = string.Empty;
            _thumbnail.texture = null;
        }
        
        public class Pool : MonoMemoryPool<Transform, SubtopicItemView>
        {
            protected override void Reinitialize(Transform viewParent, SubtopicItemView view)
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