using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class EarthPositionChooserButton : MonoBehaviour
    {

        //the Simulation Mode Toggle. It's used here to Show or Hide the Rotation or
        //Movement Mode Button. When Manual Mode is turned ON, the Rotation and Movement 
        //button appears on the screen
        [SerializeField] private Toggle _simulationModeToggle;
        //to know whether currently location choosing is enabled
        private bool isChoosingPositionOnEarth;
        //to hide and show it when needed
        [SerializeField] RotationOrMovementButton rotationOrMovementButton;
        //to Enable and Disable location choosing when the button is clicked
        [SerializeField] EarthPositionChooserController earthPositionChooserController;
        //used to Turn ON and OFF the MovementAndRotation Mode
        [SerializeField] MovementOrRotationModeController movementOrRotationModeController;

        private void OnEnable()
        {
            isChoosingPositionOnEarth = false;
        }

        //changes the buttons color to white and closes it when simulation mode
        //turns ON and opens the button when simulation mode ENDS
        public void ShowOrHideEarthPositionChooserButton()
        {
            //when Simulation ENDS
            if (!_simulationModeToggle.isOn)
            {
                //showing the button
                gameObject.SetActive(true);
            }
            //when Simulation STarts
            else
            {
                //changing the color back to white
                gameObject.GetComponent<Image>().color = Color.white;
                //closing the button
                gameObject.SetActive(false);
            }
        }

        //handles button click events
        public void ChoosePositionOnEarthButtonClicked()
        {
            //if right now is NOT choosing a position, then it needs to be ENABLED
            if (!isChoosingPositionOnEarth)
            {
                //enabling
                isChoosingPositionOnEarth = true;
                //changing button image color to highlight it as enabled
                gameObject.GetComponent<Image>().color = new Color(0, 196, 255, 255);
                //hides Rotation or Movement button to show the Find Eclipses button
                //in its place
                rotationOrMovementButton.HideRotationOrMovementModeButton();
                //Turning off the Rotation or Movement Mode
                movementOrRotationModeController.TurnOffMovementOrRotationMode();
                //disabling the sphere collider on Earth so that a position can be chosen
                earthPositionChooserController.EnablePositionChoosing();
            }
            //if right now is  choosing a position, then it needs to be DISABLED
            else
            {
                //disabling
                isChoosingPositionOnEarth = false;
                //changing the button's image color back to initial(neutral)
                gameObject.GetComponent<Image>().color = Color.white;
                //showing the Rotation or Movement Mode button
                rotationOrMovementButton.ShowRotationOrMovementModeButton();
                //setting the Rotation or Movement Mode to eithet Rotation
                //or Movement based on the configurations in the ELSE block
                //of the SwitchMovementOrRotationMode method
                movementOrRotationModeController.SwitchRotationOrMovementMode();
                //enabling the sphere collider so that Earth can be moved or rotated
                earthPositionChooserController.DisablePositionChoosing();
            }
        }

        //public void UnclickChoosePositionOnEarthButton()
        //{
        //    gameObject.GetComponent<Image>().color = Color.white;
        //}

        //used to check whether a position can be chosen on Earth
        public bool IsAllowedToChoosePositionOnEarth()
        {
            return isChoosingPositionOnEarth;
        }
    }
}
