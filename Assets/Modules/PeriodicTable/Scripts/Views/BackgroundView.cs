using UnityEngine;
using UnityEngine.UI;

namespace Module.PeriodicTable
{
    public class BackgroundView : MonoBehaviour
    {
        private bool _isСlickable = false;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void OnMouseDown()
        {
            if (_isСlickable)
            {
                SetСlickable(false);
                EventController.ClickOnBackground();
            }
        }

        public void SetVisibility(bool state)
        {
            _image.color = state ? Color.black : Color.clear;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public void SetСlickable(bool state)
        {
            _isСlickable = state;
        }
    }
}