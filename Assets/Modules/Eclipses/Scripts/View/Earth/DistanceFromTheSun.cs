using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Module.Eclipses
{
    using static DistanceFromTheSunTextController;
    public class DistanceFromTheSun : MonoBehaviour
    {
        //this variable hold the current distance from the sun.
        //it's used to display the distance on the Distance From The Sun Text
        public double distanceFromTheSun { get; private set; }
        //this is used to get the current Day Number Of The Year which is
        //used in a formula to calculate Earth's distance from the Sun
        [SerializeField] private DateController dateController;
        //referencing the Text which contains a dropdown menu with a chosen 
        //measurement unit. The method which calculates the distance uses the chosen option
        //from the dropdown menu to calculate the distance in the proper Measurement Unit
        [SerializeField] private DistanceFromTheSunTextController distanceFromTheSunTextController;

        void Update()
        {
            //updating the distance from the Sun to keep it up to date
            distanceFromTheSun = CalculateDistanceFromTheSun(distanceMeasurementUnits: distanceFromTheSunTextController.distanceMeasurementUnit);
        }

        //this method uses a special formula to calculate the distance between Earth and the Sun
        //it first estimates the distance in Astronomical Units and then , if needed, converts
        //that distance into a different measurement unit (miles, kilometers)

        private double CalculateDistanceFromTheSun(string distanceMeasurementUnits)
        {
            //this is the current day of the year. From 0 to 365
            double dayNumberOfYear = dateController.CalculateDayNumberBasedOnAngleAroundSun(getPreciseValue: true);
            //distance of Earth from the Sun in Astronomical Units
            double distance = (double)(1f - 0.01672f * Mathf.Cos((float)((0.9856 * (dayNumberOfYear - 4)) * 0.0174533)));

            //converting Aztronomical Units to Kilometers if needed(1 AU is 149597871f kilometers)
            if (distanceMeasurementUnits == DistanceMeasurementUnits.km.ToString())
            {
                distance = (int)(distance * 149597871f);
            }
            //converting Aztronomical Units to Miles if needed(1 AU is 92955807 miles)
            else if (distanceMeasurementUnits == DistanceMeasurementUnits.mi.ToString())
            {
                distance = (int)(distance * 92955807);
            }
            //Astronomical Units
            else
            {
                //cutting some digits after the decimal point so that the value 
                //in Astronomical Units is not TOO long
                distance = Mathf.Round((float)distance * 10000000.0f) / 10000000.0f;
            }

            return distance;
        }
    }
}
