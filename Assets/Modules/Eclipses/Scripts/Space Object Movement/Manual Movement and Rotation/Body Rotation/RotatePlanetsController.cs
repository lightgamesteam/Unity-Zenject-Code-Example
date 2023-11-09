using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{

    public class RotatePlanetsController : MonoBehaviour
    {
        private float _sensitivity;
        private Vector3 _mouseReference;
        private Vector3 _mouseOffset;
        private Vector3 _rotation;
        private bool _isRotating;
        private string nameObject;

        //used to check whether the RotationAndMovementMode is set to Rotation in
        //order to decide whether the objects should be rotated 
        [SerializeField] private MovementOrRotationModeController movementOrRotationModeController;

        //used to check whether space is being manually reset to simulate an eclipse chosen from the list of eclipses
        [SerializeField] private SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        void Start()
        {
            //initial setup
            nameObject = name;
            _sensitivity = 0.4f;
            _rotation = Vector3.zero;
        }

        void Update()
        {
            RotateObjectAroundAxis();
        }

        private void RotateObjectAroundAxis()
        {
            //if RotationOrMovement Mode is set to Rotation and _isRotating == true and the Clicks
            //do not happen over UI elements(Joysticks, Panel) and also when 
            //the Simulation Mode is Manual(during Automatic SImulation the operations should not happen)
            if (UIElementTouchChecker.IsPointerOverUIElement())
            {
                _isRotating = false;
            }
            if (SimulationMode.IsGameModeManual() && setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse == false && !UIElementTouchChecker.IsPointerOverUIElement() && _isRotating && movementOrRotationModeController.rotationOrMovementMode == MovementOrRotationModeController.RotationOrMovementMode.rotation && nameObject != "Moon")
            {
                // offset
                _mouseOffset = (Input.mousePosition - _mouseReference);
                //Debug.Log(_mouseOffset);
                // apply rotation
                _rotation.y = -_mouseOffset.x * _sensitivity;

                _rotation.x = _mouseOffset.y * _sensitivity;

                Vector3 rots = new Vector3(_rotation.x, _rotation.y, 0f);

                // rotate
                transform.Rotate(Vector3.ClampMagnitude(rots, 10f), Space.World);

                // store mouse
                _mouseReference = Input.mousePosition;
            }
            else if (SimulationMode.IsGameModeManual() && setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse == false && !UIElementTouchChecker.IsPointerOverUIElement() && _isRotating && movementOrRotationModeController.rotationOrMovementMode == MovementOrRotationModeController.RotationOrMovementMode.rotation && nameObject == "Moon")
            {
                // offset
                _mouseOffset = (Input.mousePosition - _mouseReference);

                // apply rotation
                _rotation.y = -_mouseOffset.x * _sensitivity;

                _rotation.x = _mouseOffset.y * _sensitivity;

                Vector3 rots = new Vector3(_rotation.x, _rotation.y, 0f);
                // rotate
                transform.Find("Moon").Rotate(Vector3.ClampMagnitude(rots, 10f), Space.World);

                // store mouse
                _mouseReference = Input.mousePosition;
            }
        }

        void OnMouseDown()
        {
            // rotating flag
            _isRotating = true;

            // store mouse
            _mouseReference = Input.mousePosition;
        }

        void OnMouseUp()
        {
            // rotating flag
            _isRotating = false;
        }
    }
}
