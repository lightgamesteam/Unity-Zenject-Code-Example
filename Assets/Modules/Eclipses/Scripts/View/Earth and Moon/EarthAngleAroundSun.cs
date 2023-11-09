using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class EarthAngleAroundSun : SpaceModuleElement
    {
        //earth's angle aroundd the Sun, from 0 to 360
        public static float earthAngleAroundSun { get; private set; }

        void Update()
        {
            //saving the angle in order for it to be accessed from outside the script
            earthAngleAroundSun = EarthAngleAroundTheSun();
        }

        //this method returns the Angle at which Earth is currently positioned relative to the Sun(values from 0 to 360)
        public float EarthAngleAroundTheSun()
        {
            var rotationAngle = app.view._planetsView.gameObject.transform.eulerAngles.y;
            if (rotationAngle < 0)
            {
                rotationAngle = 360 - Mathf.Abs(rotationAngle);
            }
            else
            {
                rotationAngle = 360 - rotationAngle;
            }
            return rotationAngle;
        }

        //CalculateAngleAroundSunBasedOnNumberOfDay(numbOfDay: CalculateDayNumberBasedOnAngleAroundSun(getPreciseValue: true));
        //takes the Number Of Day Of The Year and returns the angle at which
        //Earth is currently positioned around the Sun (between 0 and 360 degrees)
        public float CalculateAngleAroundSunBasedOnNumberOfDay(float numbOfDay)
        {
            float angleOfEarthAroundTheSun = numbOfDay * 0.98561009f;
            angleOfEarthAroundTheSun -= DateController.startDay;
            angleOfEarthAroundTheSun += 2.47509193f;
            return angleOfEarthAroundTheSun;
        }
    }
}
