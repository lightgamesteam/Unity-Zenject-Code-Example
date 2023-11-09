using TMPro;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class GradeMenuItemView : ViewBase
    {
        [SerializeField] 
        private TextMeshProUGUI _gradeName;
    
        [SerializeField] 
        private GameObject _subjectsContent;

        public TextMeshProUGUI Title
        {
            get => _gradeName;
        }

        public Transform GetSubjectsContent()
        {
            return _subjectsContent.transform;
        }

        public class Factory : PlaceholderFactory<GradeMenuItemView>
        {
        }
    }   
}