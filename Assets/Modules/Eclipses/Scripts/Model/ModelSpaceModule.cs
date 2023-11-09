using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class ModelSpaceModule : SpaceModuleElement
    {
        //this is going to be controller from within the game using a slider
        //the speed at which the planet Earth rotates around the Sun. the value of
        // 1 means that Earth rotates 1 degree around the Sun for 1 second
        [Space(6)]
        public float earthRotationSpeedAroundSun = 1f;

        //this value determines how much faster Earth rotates around its axis than
        // it does around the Sun. the value of 365.256 means that during a single
        //rotation around the Sun Earth does 365.256 rotations around its axis
        [Space(6)]
        public float earthRotationRelativeSpeedAroundAxis = 182.628f;

        //the Moon makes one full rotation around Earth in 27.322 days. This means that 
        //the Moon rotates 13.3685675 times faster around Earth than Earth does around the Sun
        //in order to synchronize Earth's movement around the Sun and Moon's movement around 
        //Earth so that they both depend on Earth's movement around the Sun the variable 
        // "moonRotationSpeedAroundEarth" should be assigned the value of 13.3685675f
        [Space(6)]
        public float moonRotationSpeedAroundEarth = 13.36877f;
    }
}
