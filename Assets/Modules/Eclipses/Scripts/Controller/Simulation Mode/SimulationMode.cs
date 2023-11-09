using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class SimulationMode : MonoBehaviour
    {

        //this variable will control the game states
        public static ModeOfSimulation simulationMode;

        //the toggle which is used to switch between automatic and manual simulation
        [SerializeField] private Toggle _simulationModeToggle;

        //ResetSolarSystem script which will be used in this script to save 
        //objects' positions in scene when the simulation mode changed to 
        //manual and later reset those positions
        [SerializeField] private ResetSolarSystem resetSolarSystem;

        //used to reset camersa position after the Simulation Mode changes back to Automatic from Manual
        [SerializeField] private CameraAutomaticController cameraAutomaticController;

        //used to cancel Space Manual Reset after Simulation starts
        [SerializeField] private SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        private void Awake()
        {
            //setting simulation mode based on the value of the Simulation
            //Mode Toggle (always used the inspector's configurations at launch time)
            //SwitchSimulationMode();
        }

        //this method will be used to switch between Game States
        //it will also control resetting space back to its initial state
        //in terms of all the objects' positions(it will start and end resetting
        //the Sun's , Earth's, and the Moon's positions)
        public void SwitchSimulationMode()
        {
            switch (_simulationModeToggle.isOn)
            {
                case true:

                    simulationMode = ModeOfSimulation.automatic;
                    //if currently space is being configured to simulate a chosen eclipse, then 
                    //it needs to be stopped so that Automatic Simulation starts and works correctly
                    if (setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse == true)
                    {
                        //stopping eclipse simulation
                        setupViewAfterEclipseChosen.EndSettingUpSceneToSimulateEclipse();
                    }
                    //when the Simulation mode changes to automatic, the objects
                    //in space should be reset to their last saved positions
                    //because they have been moved away from their original positions
                    //this method starts resetting the space objects to their
                    //last saved poisitions
                    //passing TRUE because after the reset has finished 
                    //space simulation will start immediately
                    resetSolarSystem.StartResettingSpace(isSimulationStarting: true);
                    //resetting the camera to its last position in AUTOMATIC MODE
                    cameraAutomaticController.ChangeCameraPosition(cameraPos: cameraAutomaticController.cameraPosition);
                    break;
                case false:
                    simulationMode = ModeOfSimulation.manual;
                    //here all the objects' positions need to be saved
                    //in order for the app to be able to reset those positions
                    //later if the Simulation Mode changes to automatic
                    //or the reset button is clicked from within the Options panel
                    resetSolarSystem.SaveObjectPositionAndRotationForFutureReset();
                    //just in case the Simulation Mode changes back to manual
                    //and the reset behavior is still being conducted it should
                    //be manually stopped so that the objects become stationary again
                    resetSolarSystem.EndResettingSpace();
                    break;
                default:
                    break;
            }
        }

        //this checks whether the game is set to MANUAL mode or not
        public static bool IsGameModeManual()
        {
            if (simulationMode == ModeOfSimulation.manual)
            {
                return true;
            }
            return false;
        }

        //these are the game states. Planets can move either automatically or manually
        public enum ModeOfSimulation
        {
            automatic,
            manual,
        }

        private void OnDestroy()
        {
            if (simulationMode == ModeOfSimulation.manual)
            {
                simulationMode = ModeOfSimulation.automatic;
            }
        }
    }
}
