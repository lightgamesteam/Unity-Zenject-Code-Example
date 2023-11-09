using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class PlanetsView : SpaceModuleElement
    {
        //used to check whether space is Manually simulating an eclipse
        [SerializeField] SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        private void Update()
        {
            RotateEarthAndMoonAroundTheSun();
        }

        private void RotateEarthAndMoonAroundTheSun()
        {
            //moving Earth and the Moon around the Sun when Simulation Mode is turned on and when
            //the objects in space are not being lerped(reset) to their positions
            if (!SimulationMode.IsGameModeManual() && !ResetSolarSystem.shouldResetSpaceToLastSavedPositions)
            {
                //this rotates the planet around the Sun 
                app.controller.RotatePlanetsAroundSun();
            }
            //moving Earth and the Moon around the Sun when space is Manually simulating an eclipse
            //the objects in space are not being lerped(reset) to their positions
            else if (setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse && !ResetSolarSystem.shouldResetSpaceToLastSavedPositions)
            {
                //this rotates the planet around the Sun 
                app.controller.RotatePlanetsAroundSun();
            }
        }
    }
}
