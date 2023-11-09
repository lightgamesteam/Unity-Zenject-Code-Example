using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class SubjectMenuItemView : ViewBase, ISelectableMenuItem
    {
        [Header("Normal Color")] 
        public ColorBlock normalColorBlock = ColorBlock.defaultColorBlock;

        [Header("Selected Color")] 
        public ColorBlock selectedColorBlock = ColorBlock.defaultColorBlock;

        public int ParentId { get; set; }
        public int Id { get; set; }

        [SerializeField] private TextMeshProUGUI _subjectName;
        [SerializeField] private Button _subjectButton;

        private bool _isSelected;

        public override void SubscribeOnListeners()
        {
            _subjectButton.onClick.AddListener(OnSubjectClick);
        }

        public override void UnsubscribeFromListeners()
        {
            _subjectButton.onClick.RemoveAllListeners();
        }

        private void OnSubjectClick()
        {
            Signal.Fire(new SubjectMenuItemClickViewSignal(ParentId, Id));
        }

        public void SetSubjectName(string subject)
        {
            _subjectName.text = subject;
        }

        public void Select()
        {
            _isSelected = true;
            SetHoverEnter();
        }

        public void Deselect()
        {
            _isSelected = false;
            SetHoverExit();
        }

        public bool IsSelected()
        {
            return _isSelected;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isSelected)
            {
                SetHoverEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isSelected)
            {
                SetHoverExit();
            }
        }
        
        void Update()
        {
            if (_isSelected)
                return;
        
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                SetHoverExit();
            }
        }

        private void SetHoverEnter()
        {
            _subjectButton.colors = selectedColorBlock;
            SetSubjectNameColor(Color.black);
        }

        private void SetHoverExit()
        {
            _subjectButton.colors = normalColorBlock;
            SetSubjectNameColor(Color.white);
        }

        private void SetSubjectNameColor(Color color)
        {
            _subjectName.color = color;
        }

        public class Factory : PlaceholderFactory<SubjectMenuItemView>
        {
        }
    }
}