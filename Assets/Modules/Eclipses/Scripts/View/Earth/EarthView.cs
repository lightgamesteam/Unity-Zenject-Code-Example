using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class EarthView : SpaceModuleElement
    {
        //used to check whether space is Manually simulating an eclipse
        [SerializeField] SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        private void Update()
        {
            //rotating Earth
            ControlEarthRotation();
        }

        //this method controls the rotation and movement of Earth and its axis
        public void ControlEarthRotation()
        {
            //simulating Earth's movement when Simulation Mode is turned on and when
            //the objects in space are not being lerped(reset) to their positions
            if (SimulationMode.IsGameModeManual() == false && ResetSolarSystem.shouldResetSpaceToLastSavedPositions == false)
            {
                //this rotates the planet around its own axis
                app.controller.RotateEarthAroundAxis();

                //facing Earth's axis at the same direction at all times in order to simulate the
                //alteration of SEASONS on the planet
                app.controller.FaceEarthAxisAtConstantPoint();
            }
            //simulating Earth's movement when Space is Manually simulating an eclipse
            //the objects in space are not being lerped(reset) to their positions
            else if (setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse == true && ResetSolarSystem.shouldResetSpaceToLastSavedPositions == false)
            {
                //this rotates the planet around its own axis
                app.controller.RotateEarthAroundAxis();

                //facing Earth's axis at the same direction at all times in order to simulate the
                //alteration of SEASONS on the planet
                app.controller.FaceEarthAxisAtConstantPoint();
            }
        }
    }
}
