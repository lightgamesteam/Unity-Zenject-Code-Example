using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class PinController : MonoBehaviour
    {
        [SerializeField] private GameObject _pin;

        public Vector3 localPosition;

        public void ShowPinOnEarth(float x, float y, float z)
        {
            if (!_pin.activeSelf)
            {
                EnablePin();
            }
            _pin.transform.localPosition = new Vector3(x, y, z);
        }

        private void EnablePin()
        {
            _pin.SetActive(true);
        }
    }
}
