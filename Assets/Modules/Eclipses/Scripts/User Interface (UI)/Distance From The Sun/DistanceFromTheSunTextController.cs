using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class DistanceFromTheSunTextController : MonoBehaviour
    {

        //will access the distance at which Earth curently is from the Sun
        [SerializeField] private DistanceFromTheSun distanceFromTheSun;
        //this is the text that will be shows in the top right corner
        //this can be changed from inside the editor
        [SerializeField] private string distanceFromTheSunCaption;
        //this is the Text component of the distanceFromTheSunText GameObject
        private Text distanceFromTheSunText;

        /*DON'T DELETE PLEASE
        //[Serializable] //DON'T DELETE DON'T DELETE DON'T DELETE DON'T DELETE DON'T DELETE DON'T DELETE DON'T DELETE DON'T DELETE
        //private class DistanceMeasurementUnits
        //{
        //    public bool kilometers = true;
        //    public bool miles;
        //    public bool astronomicalUnits;
        //}
        //[SerializeField] private DistanceMeasurementUnits distanceMeasurementUnits = new DistanceMeasurementUnits();
        */

        //this is a dropdown menu which will appear in the editor
        //it is used to Choose the Distance Measurement Unit in which the distance will be shown
        public enum DistanceMeasurementUnits
        {
            km, mi, AU
        }
        //showing the DropDown Menu in the editor so that the Unit can be chosen
        [SerializeField] private DistanceMeasurementUnits distanceMeasurementUnits;

        //will save the chosen Unit in a string variable and access it
        public string distanceMeasurementUnit { get; private set; }

        private void Awake()
        {
            //getting the Text component to set the text
            distanceFromTheSunText = gameObject.GetComponent<Text>();
            //getting the chosen Measurement Unit from inside the editor and saving it in a string
            distanceMeasurementUnit = distanceMeasurementUnits.ToString();
        }

        void Update()
        {
            //updating the Distance Text
            UpdateDistanceFromTheSunText();
        }

        //this method shows the distance from the sun, measurement units and a TEXT input from the editor
        private void UpdateDistanceFromTheSunText()
        {
            //getting the current distance
            var distance = distanceFromTheSun.distanceFromTheSun.ToString();
            //showing the text from the editor, the distance, and the measurement unit
            distanceFromTheSunText.text = $"{distanceFromTheSunCaption} {distance} {distanceMeasurementUnit}";
        }
    }
}
