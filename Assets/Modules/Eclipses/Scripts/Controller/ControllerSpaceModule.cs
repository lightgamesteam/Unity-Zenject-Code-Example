using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class ControllerSpaceModule : SpaceModuleElement
    {
        public Transform _earthAndMoonTransform;
        public Transform _earthTransform;
        public Transform _moonOrbit;
        public Transform _moonParentTransform;

        private void Awake()
        {
            _earthAndMoonTransform = app.view._planetsView.gameObject.transform;
            _earthTransform = app.view._earthView.gameObject.transform;
        }

        //this method rotates Earth around Sun depending on a speed specified in the inspector
        public void RotatePlanetsAroundSun(float simulationDirection = 1)
        {
            _earthAndMoonTransform.Rotate(new Vector3(0, -1, 0f) * simulationDirection, Time.deltaTime * app.model.earthRotationSpeedAroundSun);
            //Debug.Log(ShowPlanetsPositionAroundSun());
        }

        //this method rotates Earth around its axis depending on the speed of its rotation
        //around the Sun and the relative rotation speed around its axis
        public void RotateEarthAroundAxis(float simulationDirection = 1)
        {
            _earthTransform.Rotate(new Vector3(0, -1, 0) * simulationDirection, Time.deltaTime * app.model.earthRotationSpeedAroundSun * app.model.earthRotationRelativeSpeedAroundAxis, Space.Self);
        }

        //this method helps keep Earth's axis constantly facing at a specific point
        //this behavior helps simulate the alteration of SEASONS 
        public void FaceEarthAxisAtConstantPoint(float simulationDirection = 1)
        {
            //rotates Earth's axis n unison with Earth moving around the Sun
            _earthTransform.Rotate(new Vector3(0, 1, 0) * simulationDirection, Time.deltaTime * app.model.earthRotationSpeedAroundSun, Space.World);
        }

        //rotates Moon around Earth
        public void RotateMoonAroundEarth(float simulationDirection = 1)
        {
            _moonOrbit.Rotate(new Vector3(0, 0, -1) * simulationDirection, Time.deltaTime * app.model.earthRotationSpeedAroundSun * app.model.moonRotationSpeedAroundEarth, Space.Self);
        }

        //this method works like the one for Earth. It keeps the planet's rotation orbit facing in 
        //the same direction throughout the whole simulation cycle
        public void FaceMoonOrbitAtConstantPoint(float simulationDirection = 1)
        {
            _moonParentTransform.Rotate(new Vector3(0, 1, 0) * simulationDirection, Time.deltaTime * app.model.earthRotationSpeedAroundSun, Space.World);
        }
    }
}
