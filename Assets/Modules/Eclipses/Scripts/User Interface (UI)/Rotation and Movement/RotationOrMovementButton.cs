using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{

    public class RotationOrMovementButton : MonoBehaviour
    {
        //the sprites which will be interexchanged when the Rotation or Movement mode changes
        [SerializeField] private Sprite _movementModeSprite;
        [SerializeField] private Sprite _rotationModeSprite;

        //the script where the Rotation and Movement Mode is controller
        //it's used here to invoke Mode Switch Actions when the button is clicked
        [SerializeField] private MovementOrRotationModeController movementOrRotationModeController;

        //the Simulation Mode Toggle. It's used here to Show or Hide the Rotation or
        //Movement Mode Button. When Manual Mode is turned ON, the Rotation and Movement 
        //button appears on the screen
        [SerializeField] private Toggle _simulationModeToggle;

        //reference to the Image component of the button used to change its sprite
        //based on the active mode in the Rotation and Movement Controller
        private Image rotationOrMovementButtonImage;

        private void Awake()
        {
            //getting the Image component of the button to change its Sprite later
            rotationOrMovementButtonImage = GetComponent<Image>();
        }

        //when the button appears on the screen, its sprite is being set based on the default Mode
        private void OnEnable()
        {
            ShowRotationOrMovementMode(shouldSwitchMode: false);
        }

        //this method Shows the corresponding Sprite on the button based on the currently active Mode
        //the shouldSwitchMode parameter is used to determine whether the sprite should be changed
        //or not. When this method is called at Launch Time, the Sprite remains the same.
        //when the method is triggered after a Mode change has taken place, the sprite changes based
        //on the value of this parameter
        //this method is called from inside the inspector under the Rotation or Movement Mode Button's
        //OnClick Event(when the mode changes) and in the OnEnable method when the object becomes active

        public void ShowRotationOrMovementMode(bool shouldSwitchMode = true)
        {
            //if Rotation Mode is active
            if (movementOrRotationModeController.rotationOrMovementMode == MovementOrRotationModeController.RotationOrMovementMode.rotation)
            {

                //setting Rotation Mode Sprite
                rotationOrMovementButtonImage.sprite = _rotationModeSprite;

                //if the Mode changes
                if (shouldSwitchMode == true)
                {

                    //switching the Mode in the Rotation and Movement Controller
                    movementOrRotationModeController.SwitchRotationOrMovementMode();
                    //changing the sprite according to the Mode
                    rotationOrMovementButtonImage.sprite = _movementModeSprite;
                }
            }
            //if Movement Mode is active
            else if (movementOrRotationModeController.rotationOrMovementMode == MovementOrRotationModeController.RotationOrMovementMode.movement)
            {

                //setting Movement Mode Sprite
                rotationOrMovementButtonImage.sprite = _movementModeSprite;

                //if the Mode changes
                if (shouldSwitchMode == true)
                {

                    //switching the Mode in the Rotation and Movement Controller
                    movementOrRotationModeController.SwitchRotationOrMovementMode();
                    //changing the sprite according to the Mode
                    rotationOrMovementButtonImage.sprite = _rotationModeSprite;
                }
            }
        }



        //this method Shows and Hides the button based on the Simulation Mode
        //the button is Shown when on Manual Mode and Hidden when on Automatic Mode
        public void ShowOrHideRotationOrMovementModeButton()
        {
            if (!_simulationModeToggle.isOn)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }



        public void ShowRotationOrMovementModeButton()
        {
            gameObject.SetActive(true);
        }



        public void HideRotationOrMovementModeButton()
        {
            gameObject.SetActive(false);
        }
    }
}
