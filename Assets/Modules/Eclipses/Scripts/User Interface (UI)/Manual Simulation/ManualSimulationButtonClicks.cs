using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class ManualSimulationButtonClicks : MonoBehaviour
    {

        //will be used to control MANUAL simulation by button clicks
        //which will invoke the methods below from inside the inspector
        [SerializeField] private ManualSimulationController manualSimulationController;

        //this boolean values will be used from inside the Manual Simulation Controller
        //to know whether space should be Simulated Manually or not
        public bool shouldSimulateBackward { get; private set; } = false;
        public bool shouldSimulateForward { get; private set; } = false;

        //BACKWARD SIMULATION
        //the user PRESSES the button and the bool is set to TRUE
        //so that Manual Simulation works
        public void StartSimulatingManuallyBackward()
        {
            //this step is done to avoid potential BUGS(just in case)
            StopSimulatingManuallyForward();

            //this functionality should work only on Manual Mode to prevent BUGS
            if (SimulationMode.IsGameModeManual())
            {
                //allowing to Simulate Manually BACKWARD
                shouldSimulateBackward = true;
            }
        }

        //the user RELEASES the button and the bool is set to FALSE
        //so that Manual Simulation works
        public void StopSimulatingManuallyBackward()
        {
            //this functionality should work only on Manual Mode to prevent BUGS
            if (SimulationMode.IsGameModeManual())
            {
                //forbidding to Simulate Manually BACKWARD
                shouldSimulateBackward = false;
            }
        }

        //FORWARD SIMULATION
        //the user PRESSES the button and the bool is set to TRUE
        //so that Manual Simulation works
        public void StartSimulatingManuallyForward()
        {
            //this step is done to avoid potential BUGS(just in case)
            StopSimulatingManuallyBackward();

            //this functionality should work only on Manual Mode to prevent BUGS
            if (SimulationMode.IsGameModeManual())
            {
                //allowing to Simulate Manually FORWARD
                shouldSimulateForward = true;
            }
        }

        //the user RELEASES the button and the bool is set to FALSE
        //so that Manual Simulation works
        public void StopSimulatingManuallyForward()
        {
            //this functionality should work only on Manual Mode to prevent BUGS
            if (SimulationMode.IsGameModeManual())
            {
                //forbidding to Simulate Manually FORWARD
                shouldSimulateForward = false;
            }
        }

        //this will be used to SHOW the Manual Simulation Buttons once an eclipse 
        //has been Manually Set Up after being chosen from the list of found eclipses
        public void ShowManualSimulationButtonsWithDelay()
        {
            Invoke("ShowManualSimulationButtons", 2);
        }
        public void ShowManualSimulationButtons()
        {
            gameObject.SetActive(true);
        }
    }
}
