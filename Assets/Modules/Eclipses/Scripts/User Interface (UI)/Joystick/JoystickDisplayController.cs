using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class JoystickDisplayController : MonoBehaviour
    {
        //the animator containing the Appearing and Disappearing clips
        [SerializeField] private Animator joystickAnimator;

        //the toggle which turns on or off the simulation mode
        [SerializeField] private Toggle _simulationModeToggle;

        //this method is called whenever the Simulation Mode Toggle's value is being changed. It
        //is set up in the inspector unde the "OnValueCHanged" field. 
        //the method shows the joysticks when MANUAL mode is turned on and hides them when AUTOMATIC
        //simulation starts
        public void JoystickDisplay()
        {
            //if the toggle is turned off and MANUAL mode starts
            if (!_simulationModeToggle.isOn)
            {
                //showing the joysticks
                joystickAnimator.SetTrigger("JoystickAppearing");
            }
            //if the toggle is turned on and AUTOMATIC simulation starts
            else
            {
                //hiding the joysticks
                joystickAnimator.SetTrigger("JoystickDisappearing");
            }
        }
    }
}