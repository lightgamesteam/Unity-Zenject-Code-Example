using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class RotationJoystickController : MonoBehaviour
    {
        //referencing the Joystick to get its input
        [SerializeField] private FixedJoystick _rotationJoyStick;
        //this us going to be accessed from within the camera's mechanical controller toi control camera's rotation
        public static Vector2 rotationJoystickInput;

        private void Awake()
        {
            //init
            rotationJoystickInput = new Vector2();
        }

        private void Update()
        {
            //saving the Joystick's input in an accessible variable
            rotationJoystickInput = GetRotationJoystickInput();
        }

        //this method returns a Vector2 with the joystick's input
        public Vector2 GetRotationJoystickInput()
        {
            return new Vector2(_rotationJoyStick.Horizontal, _rotationJoyStick.Vertical * (-1));
        }
    }
}
