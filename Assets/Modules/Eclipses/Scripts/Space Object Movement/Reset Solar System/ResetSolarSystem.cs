using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class ResetSolarSystem : MonoBehaviour
    {
        //this will be used to enable and disable the toggle
        //when there are active lerp operations, the toggle
        //will be disabled in order to avoid some bugs
        [SerializeField] private SimulationModeToggleController simulationModeToggleController;

        //the objects in solar system whose positions should be reset
        [SerializeField] private GameObject _parentEarth;
        [SerializeField] private GameObject _parentMoon;
        [SerializeField] private GameObject _parentSun;
        //when space is being reset the pin moves off 
        //Earth's surface. So this reference is used to bring it back
        [SerializeField] private GameObject _pin;

        //variables which will be used to store each objects position in scene 
        //when the game mode was switched to MANUAL from automatic
        private Vector3 parentEarthLocalPosition;
        private Vector3 parentMoonLocalPosition;
        private Vector3 parentSunLocalPosition;

        //rotations of objects to be saved
        private Vector3 earthRotation;
        private Vector3 moonRotation;
        private Vector3 sunRotation;

        //this boolean value will be used to decide whether the reset behavior 
        //should be fulfilled at a particular moment or not
        public static bool shouldResetSpaceToLastSavedPositions = false;

        //this is the minimum distance at which the lerp operations
        //will be overridden by manually setting the objects' positions
        //which has to be done since LERP does not work precisely
        //and objects do not reach their destinations in the end
        private readonly float minDistanceToOverrideResetLerp = 0.5f;

        private void Awake()
        {
            //making sure there are no NULL REFERENCES in the life cycle of the app
            //by checking and manually finding objects which are null at the moment
            FindObjectsIfNeeded();

            shouldResetSpaceToLastSavedPositions = false;
        }

        private void Update()
        {
            //resetting space only if the delegate boolean value allows to do so
            //this value is changed in the StartReset and EndReset functions
            //the StartReset function is called from within the SimulationMode
            //script when the Simulation Mode changes to Automatic or from
            //the ResetButton on the Options Panel
            if (shouldResetSpaceToLastSavedPositions == true)
            {
                //this method brings all the objects in space back to their positions
                ResetSpaceToLastSaved();
            }
        }

        //this method STARTS the reset behavior by setting the boolean value
        //"shouldResetSpaceToLastSavedPositions" to TRUE
        //the parameter is passed to notify the method whether after its 
        //implementation the Automatic Simulation Mode will start
        //it is needed because when this method is called using 
        //the Reset Button on Options Panel it should also check
        //if the game is on Manual Mode. If it's on Automatic Mode
        //then the Reset Button should not invoke any action
        public void StartResettingSpace(bool isSimulationStarting)
        {

            //if Simulation Mode will turn on
            if (isSimulationStarting == true)
            {
                //disabling the Simulation Mode toggle because object will be moved in a moment
                simulationModeToggleController.DisableSimulationModeToggle();
                //space will be reset
                shouldResetSpaceToLastSavedPositions = true;
            }
            //simulation mode won't start. The code below works when
            //the method is invoked by the Reset Button on Options Panel
            else
            {
                //if Automatic Simulation is ON, the code below won't work
                //because the reset behavior MUST work only on Manual Mode
                if (SimulationMode.IsGameModeManual() == true)
                {
                    //disabling the Simulation Mode toggle because object will be moved in a moment
                    simulationModeToggleController.DisableSimulationModeToggle();
                    //space will be reset
                    shouldResetSpaceToLastSavedPositions = true;
                }
            }

            //resetting the objects rotation to last saved values
            ResetObjectRotationsToLastSaved();
        }
        //this method ENDS the reset behavior by setting the boolean value to FALSE
        //this method is called once the resetting operations are finished in the
        //ResetSpaceToLastSaved method
        public void EndResettingSpace()
        {

            shouldResetSpaceToLastSavedPositions = false;

            //enabling the toggle because the lerp operations have finished
            simulationModeToggleController.EnableSimulationModeToggle();
        }
        //this method is used to lerp all the objects in space back to their last saved positions 
        private void ResetSpaceToLastSaved()
        {

            //this variables hold distances between current positions
            //of objects in space and their destinations(positions to which)
            //they are being moved in this method using lerp
            float distanceBetweenCurrentAndEndPositionsOfSun = Mathf.Infinity;
            float distanceBetweenCurrentAndEndPositionsOfEarth = Mathf.Infinity;
            float distanceBetweenCurrentAndEndPositionsOfMoon = Mathf.Infinity;

            //the Sun
            {
                //moving the Sun
                _parentSun.transform.localPosition = Vector3.Lerp(_parentSun.transform.localPosition, parentSunLocalPosition, Time.deltaTime * 2);
                //getting the distance bettween the Sun's current position and its destination
                distanceBetweenCurrentAndEndPositionsOfSun = Vector3.Distance(_parentSun.transform.localPosition, parentSunLocalPosition);
            }
            //Earth
            {
                //moving Earth
                _parentEarth.transform.localPosition = Vector3.Lerp(_parentEarth.transform.localPosition, parentEarthLocalPosition, Time.deltaTime * 2);
                //getting the distance bettween Earth's current position and its destination
                distanceBetweenCurrentAndEndPositionsOfEarth = Vector3.Distance(_parentEarth.transform.localPosition, parentEarthLocalPosition);
            }
            //the Moon
            {
                //moving the Moon
                _parentMoon.transform.localPosition = Vector3.Lerp(_parentMoon.transform.localPosition, parentMoonLocalPosition, Time.deltaTime * 2);
                //getting the distance bettween the Moon's current position and its destination
                distanceBetweenCurrentAndEndPositionsOfMoon = Vector3.Distance(_parentMoon.transform.localPosition, parentMoonLocalPosition);
            }

            //the snippet below constantly checks whether the objects are very close
            //to their destinations. The minimum destination is specified in this script. It may be 
            //modified but the visual effects will deteriorate because there will be jittery
            //movements in the scene. The value of 0.5 is the best minimum distance to use
            //if all the distances are less than the minimum specified distance for overriding lerping operations
            if (distanceBetweenCurrentAndEndPositionsOfSun < minDistanceToOverrideResetLerp && distanceBetweenCurrentAndEndPositionsOfEarth < minDistanceToOverrideResetLerp && distanceBetweenCurrentAndEndPositionsOfMoon < minDistanceToOverrideResetLerp)
            {
                //manually resetting objects to their saved positions since lerp
                //will never bring them all the way to their destinations
                _parentSun.transform.localPosition = parentSunLocalPosition;
                _parentMoon.transform.localPosition = parentMoonLocalPosition;
                _parentEarth.transform.localPosition = parentEarthLocalPosition;

                //ending the resetting behavior. The method will not be called
                //beyond this point until the next time it's triggered
                //by the StartReset method
                EndResettingSpace();
            }
            else
            {
                //outputting the objects' distances from their destinations
                //Debug.Log($"{distanceBetweenCurrentAndEndPositionsOfSun}-----{distanceBetweenCurrentAndEndPositionsOfEarth}-----{distanceBetweenCurrentAndEndPositionsOfMoon}");
            }
        }

        //this method is used to abruptly reset Body positions in space without smooth
        //transition and without LERP
        public void ResetSpaceToLastSavedAbruptly()
        {
            //saving the localposition to position the pin again
            //after resetting positions
            var pinLocalPosition = _pin.transform.localPosition;

            //resetting positions of objects in space
            _parentSun.transform.localPosition = parentSunLocalPosition;
            _parentMoon.transform.localPosition = parentMoonLocalPosition;
            _parentEarth.transform.localPosition = parentEarthLocalPosition;

            //bringing the pon back to where it was before it flew
            //of Earth's surface while being reset
            _pin.transform.localPosition = pinLocalPosition;

            //resetting rotations of objects in space
            ResetObjectRotationsToLastSaved();
        }

        //this method reverts the rotated objects' rotations to their last saved values
        private void ResetObjectRotationsToLastSaved()
        {
            _parentEarth.transform.localEulerAngles = earthRotation;
            _parentMoon.transform.GetChild(0).transform.localEulerAngles = moonRotation;
            _parentSun.transform.localEulerAngles = sunRotation;
        }

        //this method is used to save the positions of objects in scene
        //in order to reset them to their last positions when the user
        //taps the RESET button on the Options Panel or turns on the 
        //automatic simulation mode using the Simulation Toggle button
        //in other words, this method is called from within the 
        //SimulationMode script in the SwitchSimulationMode method
        //when the Simulation Mode changes to Manual
        public void SaveObjectPositionAndRotationForFutureReset()
        {
            //saving the positions of the objects
            parentEarthLocalPosition = _parentEarth.transform.localPosition;

            parentMoonLocalPosition = _parentMoon.transform.localPosition;

            parentSunLocalPosition = _parentSun.transform.localPosition;
            //

            //saving the rotations of the objects
            earthRotation = _parentEarth.transform.localRotation.eulerAngles;

            moonRotation = _parentMoon.transform.GetChild(0).transform.localRotation.eulerAngles;

            sunRotation = _parentSun.transform.localRotation.eulerAngles;
            //
        }

        //this method checks NULL REFERENCES and fixes them 
        private void FindObjectsIfNeeded()
        {
            //if the Sun(parent) GameObject was not dragged into its corresponding field in the inspector
            //then it needs to be found manually using its tag
            if (_parentSun == null)
            {
                _parentSun = GameObject.FindWithTag("Sun");
            }
            //if the Earth(parent) GameObject was not dragged into its corresponding field in the inspector
            //then it needs to be found manually using its tag
            if (_parentEarth == null)
            {
                _parentEarth = GameObject.FindWithTag("Earth");
            }
            //if the Moon(parent) GameObject was not dragged into its corresponding field in the inspector
            //then it needs to be found manually using its tag
            if (_parentMoon == null)
            {
                _parentMoon = GameObject.FindWithTag("Moon");
            }
        }
    }
}
