using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class MoonLightController : MonoBehaviour
    {
        //references the GameObject at which the light will be looking
        [SerializeField] private GameObject _moon;
        //when simulating an eclipse the pin will be lit instead of the Moon
        [SerializeField] private GameObject _pin;

        //determines whether a light must be cast upon the Moon or the Pin
        public bool shouldCastLightUponThePin;

        void Update()
        {
            if (shouldCastLightUponThePin)
            {
                //casting a light on the Pin
                transform.LookAt(_pin.transform);
            }
            else
            {
                //casting a light on the Moon
                transform.LookAt(_moon.transform);
            }
        }

        //lighting the pin
        public void LightPinInsteadOfTheMoon()
        {
            shouldCastLightUponThePin = true;
        }
  
        //lighting the Moon
        public void RevertToLightingTheMoon()
        {
            shouldCastLightUponThePin = false;
        }
    }
}
