using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class MovementOrRotationModeController : MonoBehaviour
    {
        //accessed from outside this script, namely in RotatePlanetsController and BodyMovementController
        //to enable or disable actions over objects in scene
        [HideInInspector]
        public RotationOrMovementMode rotationOrMovementMode;
        //used to reset the sprite when the Default Rotation or Movement Mode is turned ON
        [SerializeField] private RotationOrMovementButton rotationOrMovementButton;

        private void Awake()
        {
            //initial setup of the Mode
            SwitchRotationOrMovementMode();
        }

        public void SwitchRotationOrMovementMode()
        {
            //if Rotation or Movement mode was set to Movement, then it changes to Rotation
            if (rotationOrMovementMode == RotationOrMovementMode.movement)
            {
                rotationOrMovementMode = RotationOrMovementMode.rotation;
            }
            //if Rotation or Movement mode was set to Rotation, then it changes to Movement
            else if (rotationOrMovementMode == RotationOrMovementMode.rotation)
            {
                rotationOrMovementMode = RotationOrMovementMode.movement;
            }
            //this code works at Launch Time when the rotationOrMovementMode variable is
            //empty. This can be changed to Rotation and the code will work
            else
            {
                rotationOrMovementMode = RotationOrMovementMode.movement;
                //setting the default sprite for the Default Movement or Rotation Mode (should be called with FALSE)
                rotationOrMovementButton.ShowRotationOrMovementMode(shouldSwitchMode: false);
            }
        }

        //here the Movement or Rotation Mode is neither Rotation nor Movement
        public void TurnOffMovementOrRotationMode()
        {
            rotationOrMovementMode = RotationOrMovementMode.noMode;
        }

        //the available modes while on Manual Mode
        //the player can either Rotate or Move the objects in scene
        public enum RotationOrMovementMode
        {
            rotation,
            movement,
            noMode,
        }
    }
}
