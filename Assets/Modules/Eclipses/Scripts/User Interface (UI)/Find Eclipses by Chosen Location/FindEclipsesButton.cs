using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class FindEclipsesButton : MonoBehaviour
    {
        //to know whether Simulation Mode is ON or OFF
        [SerializeField] private Toggle _simulationModeToggle;
        //used to open the scroll view when eclipses should be found
        //for a clicked location on Earth's surface
        [SerializeField] private GameObject _eclipseScrollView;
        //the pin on Earth's surface.
        [SerializeField] private GameObject _pin;

        //when Simulation Mode starts the FindEclipses button closes 
        //because this functionality should not be available during Simulation Mode
        public void HideFindEclipsesButton()
        {
            //if simulation has started
            if (_simulationModeToggle.isOn)
            {
                //closing the button
                gameObject.SetActive(false);
            }
        }

        //this method shows the button if it's hidden and hides 
        //it if it is visible on the screen
        public void ShowOrHideFindEclipsesButton()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        //open the Scroll View to show eclipses that have been found for the clicked location
        public void FindEclipsesByChosenPosition()
        {
            //if the pin is not active on the surface of Earth then the
            //eclipse list view does not open at all(if it opens when the pin is
            //inactive on Earth's surface it will cause BUGS)
            if (_pin.activeSelf)
            {
                //opening the view which shows the list of all found eclipses
                //by chosen position on Earth's surface
                _eclipseScrollView.SetActive(true);
            }
        }
    }
}
