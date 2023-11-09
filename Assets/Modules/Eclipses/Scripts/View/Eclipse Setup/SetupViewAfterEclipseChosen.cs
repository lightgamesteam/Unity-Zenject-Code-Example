using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class SetupViewAfterEclipseChosen : MonoBehaviour
    {
        //Earth to move
        [SerializeField] private EarthView _earthView;
        //the Moon to movr
        [SerializeField] private MoonView _moonView;
        //the model to set the simulation speed
        [SerializeField] private ModelSpaceModule model;
        //used to get the chosen eclipse from the list of found eclipses
        [SerializeField] EclipseChooseController eclipseChooseController;
        //used to check whether Earth has already reached its destination
        [SerializeField] DateController dateController;
        //used to determine whether space should be Manually Set Up
        public bool shouldSetupSpaceToSimulateEclipse { get; private set; } = false;
        //the speed at which space is being set up. It's set from inside the editor
        [SerializeField] private float eclipseChooseSetupSimulationSpeed = 100f;
        //used to reset everything to initial positions before Earth is moved to a chosen eclipse date
        [SerializeField] private ResetSolarSystem resetSolarSystem;
        //used to stop position choosing on Earth's surface
        [SerializeField] private EarthPositionChooserController earthPositionChooserController;
        //used to rotate Earth so that the pin reaches a point at which it's
        //visible when looked upon from the Sun's direction and a shadow
        //can be cast upon the location of the pin to simulate an eclipse
        [SerializeField] private PinDarkSideCheckerQuadController pinDarkSideCheckerQuadController;
        //the simulation speed slider adjuster. This is used to set its minimum value
        //as the overall Simulation Speed in scene so that after an eclipse is set up in scene
        //and the Simulation Button is clicked the rotations and movements happen at the lowest speed
        //possible so that the user can see how the Moon's shadow moves away from the chosen location
        [SerializeField] private Slider _simulationSpeedSlider;
        //the Manual Simulation Buttons which should
        //be opened after an eclipse has been Manually set
        //so that the user can watch the Moon's shadow move on Earth's surface
        [SerializeField] private ManualSimulationButtonClicks manualSimulationButtonClicks;
        [SerializeField] private Button earthPositionChooserButton;
        [SerializeField] private GameObject moonChild;

        void Update()
        {
            //working on Manual Mode only
            if (SimulationMode.IsGameModeManual())
            {
                //setting up scene
                SetUpSpaceToSimulateEclipse();
            }
        }

        //this method sets a speed at which space needs to be set up when an eclipse is chosen
        private void SetAppropriateSimulationSpeed(float eclipseChooseSetUpSimulationSpeed)
        {
            //working on Manual Mode only
            if (SimulationMode.IsGameModeManual())
            {
                model.earthRotationSpeedAroundSun = eclipseChooseSetUpSimulationSpeed;
            }
        }

        //this method STARTS to set up space to Manually simulate an eclipse
        public void StartSettingUpSceneToSimulateEclipse()
        {
            //working on Manual Mode only
            if (SimulationMode.IsGameModeManual())
            {
                //setting the speed
                SetAppropriateSimulationSpeed(eclipseChooseSetUpSimulationSpeed: eclipseChooseSetupSimulationSpeed);
                //starting simulation

                //first bringing everything back to its initial configurations
                resetSolarSystem.ResetSpaceToLastSavedAbruptly();

                //enabling rotation to a chosen eclipse date
                shouldSetupSpaceToSimulateEclipse = true;
            }
        }
        //this method ENDS the Manual Setup of bodies in space
        public void EndSettingUpSceneToSimulateEclipse()
        {
            //stopping Simulation
            shouldSetupSpaceToSimulateEclipse = false;
            //setting the Simulation Speed Slider's value (times X)
            //as the speed of Simulation after an eclipse has been manually set up
            //when the user chooses it from the list of found eclipses
            //(first the space is being set up at a fast speed to set positions for simulating 
            //an eclipse and then the Simulation Speed is reduced to another value)
            SetAppropriateSimulationSpeed(eclipseChooseSetUpSimulationSpeed: _simulationSpeedSlider.minValue * 10);
        }

        //this method is used to rotate Earth and the Moon and bring them
        //to the position where they are supposed to be during the eclipse
        private void SetUpSpaceToSimulateEclipse()
        {
            //checking whether should move Earth and the Moon
            if (shouldSetupSpaceToSimulateEclipse)
            {
                //moving Earth
                _earthView.ControlEarthRotation();
                earthPositionChooserButton.interactable = false;
                //moving the Moon
                _moonView.ControlMoonRotation();

                //Debug.Log($"choosen eclipse date: {TruncateString(str: eclipseChooseController.choosenEclipse.month, 3)} {eclipseChooseController.choosenEclipse.day}");
                //Debug.Log($"current date: {TruncateString(str: dateController.currentDate.month, 3)} {dateController.currentDate.day}");

                //if the chosen eclipse's dat eis the same as the date in space, the manual
                //movement of Earth and the Moon stops
                if (TruncateString(str: eclipseChooseController.choosenEclipse.month, 3) == TruncateString(str: dateController.currentDate.month, 3) && eclipseChooseController.choosenEclipse.day == (dateController.currentDate.day).ToString())
                {
                    //Earth has reached the chosen eclipse date and it no longer
                    //needs to be rotated around the sun
                    EndSettingUpSceneToSimulateEclipse();
                    //no longer allows the user to choose a position on Earth's surface
                    earthPositionChooserController.DisablePositionChoosing();
                    //calls a method which rotates Earth so that the pin
                    //over the chosen eclipse position is facing the Sun
                    pinDarkSideCheckerQuadController.CheckIfPinIsOnDarkSideWithDelay();
                    //opening the MANUAL SIMULATION BUTTONS so that the user
                    //can move FORWARD and BACKWARD in time to see how 
                    //the shadow moves over Earth's surface
                    manualSimulationButtonClicks.ShowManualSimulationButtonsWithDelay();
                    earthPositionChooserButton.interactable = true;
                    moonChild.transform.LookAt(Vector3.zero);
                }
            }
        }
        public void EnabledButtonInteractable ()
        {
            earthPositionChooserButton.interactable = true;
        }
        //this method is used to Substring the first few letters of the Month names so that
        //they can be used as a condition when stopping to set up scene to manually
        //simulate an eclipse chosen from the list
        public static string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Substring(0, Mathf.Min(str.Length, maxLength));
        }
    }
}
