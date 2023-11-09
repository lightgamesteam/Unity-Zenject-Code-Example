using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class ManualSimulationController : ControllerSpaceModule
    {

        //this is the controller which is responsible
        //for the Manual Simulation Buttons' Events
        [SerializeField] private ManualSimulationButtonClicks manualSimulationButtonClicks;
        //informs if space is being Manually Simulated using Manual SImulation Buttons(backward,forward)
        //when game is on Manual Mode
        public bool isSpaceBeingManuallySimulated;

        private void Update()
        {
            //Simulate Manually ONLY On Manual Mode
            if (SimulationMode.IsGameModeManual())
            {
                //if should simulate Manually BACKWARD
                if (manualSimulationButtonClicks.shouldSimulateBackward)
                {
                    //informing that space is being Manually Simulated
                    isSpaceBeingManuallySimulated = true;

                    //simulating Manually BACKWARD
                    SimulateSpaceBackward();
                }
                //if should simulate Manually FORWARD
                if (manualSimulationButtonClicks.shouldSimulateForward)
                {
                    //informing that space is being Manually Simulated
                    isSpaceBeingManuallySimulated = true;

                    //simulating Manually FORWARD
                    SimulateSpaceForward();
                }
            }
            //Automatic Simulation Mode
            else
            {
                //informing that space is NOT being Manually Simulated anymore
                isSpaceBeingManuallySimulated = false;
            }
        }

        //moves space BACKWARD in time
        public void SimulateSpaceBackward()
        {
            //rotating Earth and the Moon around the Sun
            RotatePlanetsAroundSun(simulationDirection: -1);
            //rotating Earth around its axis
            RotateEarthAroundAxis(simulationDirection: -1);
            //Earth constantly looks at a certain point to keep its axis facing in one direction
            FaceEarthAxisAtConstantPoint(simulationDirection: -1);
            //rotating the Moon around Earth
            RotateMoonAroundEarth(simulationDirection: -1);
            //the Moon's rotation orbit looks constantly in a specific direction
            FaceMoonOrbitAtConstantPoint(simulationDirection: -1);
        }

        //moves space FORWARD in time
        public void SimulateSpaceForward()
        {
            //rotating Earth and the Moon around the Sun
            RotatePlanetsAroundSun();
            //rotating Earth around its axis
            RotateEarthAroundAxis();
            //Earth constantly looks at a certain point to keep its axis facing in one direction
            FaceEarthAxisAtConstantPoint();
            //rotating the Moon around Earth
            RotateMoonAroundEarth();
            //the Moon's rotation orbit looks constantly in a specific direction
            FaceMoonOrbitAtConstantPoint();
        }
    }
}
