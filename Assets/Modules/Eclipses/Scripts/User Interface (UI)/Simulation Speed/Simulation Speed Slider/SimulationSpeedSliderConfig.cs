using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class SimulationSpeedSliderConfig : SpaceModuleElement
    {
        //referencing the Slider which is used to adjust the simulation speed in space
        private Slider simulationSpeedAdjuster;

        //the model of this app which will be used to access the Simulation Speed (EarthRotationSpeedAroundSun)
        //variable and Update it whenever needed
        [SerializeField] private ModelSpaceModule model;

        private void Awake()
        {
            //getting the slider component which is on the host gameObject of this script
            simulationSpeedAdjuster = gameObject.GetComponent<Slider>();
        }

        //every time the Simulation Speed Adjuster slider opens its value gets set up
        //based on the EarthRotationSpeedAroundSun variable from the app model
        private void OnEnable()
        {
            //setting its value using the rotation speed of Earth 
            //around the Sun from the Model(is set and overridden from inside the inspector)
            simulationSpeedAdjuster.GetComponent<Slider>().value = model.earthRotationSpeedAroundSun;
        }

        //this method is used to update the Simulation Speed in this app's model
        public void SpaceSimulationSpeedUpdater()
        {
            //accessing the corresponding variable in the model and assigning 
            //a new value to it based on the Adjuster's current value
            model.earthRotationSpeedAroundSun = simulationSpeedAdjuster.value;
        }
    }
}
