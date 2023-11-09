using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class SimulationSpeedText : MonoBehaviour
    {
        //the model of this app which will be used to access the Simulation Speed (EarthRotationSpeedAroundSun)
        //variable and Update it whenever needed
        [SerializeField] private ModelSpaceModule model;

        private void OnEnable()
        {
            //every time the Text apperas, its value gets updated
            UpdateSimulationSpeedText();
        }

        //method called when the Slider's value changes
        //displays the new Simulation Speed in the Text based on the Slider's value
        public void UpdateSimulationSpeedText()
        {
            gameObject.GetComponent<Text>().text = $"{Mathf.RoundToInt(360.0f / model.earthRotationSpeedAroundSun).ToString()} seconds around the Sun";
        }
    }
}
