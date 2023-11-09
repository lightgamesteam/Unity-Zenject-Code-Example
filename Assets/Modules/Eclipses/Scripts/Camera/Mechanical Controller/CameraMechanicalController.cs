using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class CameraMechanicalController : MonoBehaviour
    {

        //referencing the camera
        [SerializeField] GameObject _mainCamera;
        //referencing the Trasform if the Camera to avoid calling it all the time
        private Transform mainCameraTransform;

        //this variable controls how sharply the RotationJoystick rotates the camera
        [SerializeField] private float cameraRotationSensitivity = 1f;
        //this variable controls how sharply the MovementJoystick moves the camera Forward and Backward
        [SerializeField] private float cameraMovementSensitivity = 1f;

        private void Awake()
        {
            //if the camera is not found in the inspector, then manually finding it
            if (_mainCamera == null)
            {
                _mainCamera = gameObject;
            }
            //saving camera's transform
            mainCameraTransform = _mainCamera.transform;
        }

        // Update is called once per frame
        void Update()
        {
            //allowing to use manual controls only if the game mode is MANUAL
            if (SimulationMode.IsGameModeManual())
            {
                //rotating the camera around its X and Y axes based on the RotationJoystick's input
                RotateCamera();
                //moving the camera forward or backward based on the MovementJoystick's input
                MoveCamera();
            }
        }

        //this method rotates the camera around its X and Y axes based on th Rotation Joystick's
        //input and the Movement Modifier (cameraRotationSensitivity)
        private void RotateCamera()
        {
            //rotating the camera around its X axis(HORIZONTALLY)
            mainCameraTransform.Rotate(new Vector3(RotationJoystickController.rotationJoystickInput.y, 0, 0) * cameraRotationSensitivity, Space.Self);
            //rotating the camera around its Y axis(VERTICALLY) in Space.World, because if it is being moved around
            //its Y axis in Space.Self, its Z rotation changes for some reason.
            mainCameraTransform.Rotate(new Vector3(0, RotationJoystickController.rotationJoystickInput.x, 0) * cameraRotationSensitivity, Space.World);
        }

        //this method transforms the Camera either FORWARD or BACKWARD based on the Movement
        //joystick's input and the Sensitivity Modifier (cameraMovementSensitivity)
        private void MoveCamera()
        {
            mainCameraTransform.position += mainCameraTransform.forward * MovementJoystickController.movementJoystickInput * cameraMovementSensitivity;
        }
    }
}
