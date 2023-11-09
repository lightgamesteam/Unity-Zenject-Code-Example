using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class MoonView : SpaceModuleElement
    {
        //used to check whether space is Manually simulating an eclipse
        [SerializeField] SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        private void Update()
        {
            //controlling the Moon's behavior
            ControlMoonRotation();
        }

        //rotating the moon and controlling its orbit's facing direction
        public void ControlMoonRotation()
        {
            //simulating the Moon's movement when Simulation Mode is turned on and when
            //the objects in space are not being lerped(reset) to their positions
            if (!SimulationMode.IsGameModeManual() && !ResetSolarSystem.shouldResetSpaceToLastSavedPositions)
            {
                //this rotates the Moon around Earth 
                app.controller.RotateMoonAroundEarth();

                //face's moon at constant point
                app.controller.FaceMoonOrbitAtConstantPoint();
            }
            //simulating the Moon's movement when it should Manually Simulate an eclipse
            //the objects in space are not being lerped(reset) to their positions
            else if (setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse && !ResetSolarSystem.shouldResetSpaceToLastSavedPositions)
            {
                //this rotates the Moon around Earth 
                app.controller.RotateMoonAroundEarth();

                //face's moon at constant point
                app.controller.FaceMoonOrbitAtConstantPoint();
            }
        }
    }
}
