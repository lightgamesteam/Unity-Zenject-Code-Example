using TMPro;
using UnityEngine;

namespace TDL.Views
{
    public class TopicItemViewMobile : TopicItemView
    {
        [SerializeField]
        public GameObject Video2DIcon;
        [SerializeField]
        public TextMeshProUGUI Video2DNumber;

        public void SetVideo2D(int number)
        {
            SetTopicItem(number, Video2DIcon, Video2DNumber);
        }
        
        [SerializeField]
        public GameObject Video3DIcon;
        [SerializeField]
        public TextMeshProUGUI Video3DNumber;
    
        public void SetVideo3D(int number)
        {
            SetTopicItem(number, Video3DIcon, Video3DNumber);
        }
    
        [SerializeField]
        public GameObject Model3DIcon;
        [SerializeField]
        public TextMeshProUGUI Model3DNumber;
    
        public void SetModel3D(int number)
        {
            SetTopicItem(number, Model3DIcon, Model3DNumber);
        }
    
        [SerializeField]
        public GameObject ModulesIcon;
        [SerializeField]
        public TextMeshProUGUI ModulesNumber;
    
        public void SetModules(int number)
        {
            SetTopicItem(number, ModulesIcon, ModulesNumber);
        }
    
        private void SetTopicItem(int number, GameObject icon, TextMeshProUGUI textNumber)
        {
            icon.SetActive(number > 0);
            textNumber.gameObject.SetActive(number > 0);
            textNumber.text = number.ToString();
        }
    }
}