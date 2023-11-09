using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class MovementJoystickController : MonoBehaviour
    {
        //referencing the Joystick to get its input
        [SerializeField] private FixedJoystick _movementJoyStick;
        //this us going to be accessed from within the camera's mechanical controller toi control camera's rotation
        public static float movementJoystickInput;

        // Update is called once per frame
        void Update()
        {
            movementJoystickInput = GetMovementJoystickInput();
        }

        //this method returns a float number (the joystick's input)
        public float GetMovementJoystickInput()
        {
            return _movementJoyStick.Vertical;
        }
    }
}
