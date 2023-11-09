using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Module.Eclipses
{

    //in this script the Movement Axes are enabled whenever necessary on objects which
    //have been clicked on screen

    public class BodyMovementController : MonoBehaviour
    {
        // Change me to change the touch phase used.
        TouchPhase touchPhase = TouchPhase.Ended;

        // Objects
        private GameObject sunAxes, earthAxes, moonAxes, prev_axes;

        //used to check whether the Movement Mode is active as opposed to Rotation Mode
        [SerializeField] private MovementOrRotationModeController movementOrRotationModeController;

        //used to check whether space is being manually simulated to bring about an eclipse
        //after it has been chosen from the list of eclipses
        [SerializeField] private SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        void Update()
        {
            //checking if Simulation Mode is Manual...when 
            //the Simulation Mode is Manual(during Automatic SImulation the operations should not happen)
            //checking whether the clicked GameObject in the scene is NOT a UI element.
            //checking to make sure space is not being manually reset to simulate an eclipse chosen from the list of eclipses
            //if instead of a GameObject a UI element is clicked then GameObjects should not be clickable
            //this means that, for example, if the user wants to click on the Sun which is blocked
            //by a UI element, say, Options Panel, then the click won't work because
            //it will come straight on a UI element(Options Panel)
            if (SimulationMode.IsGameModeManual() && !setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse && !UIElementTouchChecker.IsPointerOverUIElement() && movementOrRotationModeController.rotationOrMovementMode == MovementOrRotationModeController.RotationOrMovementMode.movement)
            {
                //getting screen touches and enabling the Movement Axes on objects whenever 
                //those touches happen straight on them
                AxesDisplayer();
            }
        }

        private void AxesDisplayer()
        {
            //We check if we have more than one touch happening.
            //We also check if the first touches phase is Ended (that the finger was lifted)
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
            {
                Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;

                //sending a raycast to find out if a GameObject has been clicked
                if (Physics.Raycast(raycast, out raycastHit))
                {

                    //if the clicked GameObject is the Sun
                    if (raycastHit.collider.tag == "Sun")
                    {
                        //getting the raycastHit's transform to loop among its children
                        var raycastHitTransform = raycastHit.collider.transform;
                        //looping among the transform's children to find the "Axes"
                        //GameObject and Activate it
                        foreach (Transform axis in raycastHitTransform)
                        {
                            //found child called Axis. Now it will be enabled
                            if (axis.name == "Axes")
                            {
                                sunAxes = axis.gameObject;
                                GameObject sun = axis.parent.Find("Sun").gameObject;
                                sunAxes.transform.rotation = Quaternion.Euler(0, 0, 0);
                                ShowHideAxes(sunAxes);
                            }
                        }
                    }
                    //if the clicked GameObject is Earth
                    if (raycastHit.collider.tag == "Earth")
                    {
                        //getting the raycastHit's transform to loop among its children
                        var raycastHitTransform = raycastHit.collider.transform;
                        //looping among the transform's children to find the "Axes"
                        //GameObject and Activate it
                        foreach (Transform axis in raycastHitTransform)
                        {
                            //found child called Axis. Now it will be enabled
                            if (axis.name == "Axes")
                            {
                                earthAxes = axis.gameObject;
                                GameObject earth = axis.parent.Find("EarthAroundAxis").gameObject;
                                earthAxes.transform.rotation = Quaternion.Euler(0, 0, 0);
                                ShowHideAxes(earthAxes);
                            }
                        }
                    }
                    //if the clicked GameObject is the Moon
                    if (raycastHit.collider.tag == "Moon")
                    {
                        //getting the raycastHit's transform to loop among its children
                        var raycastHitTransform = raycastHit.collider.transform;
                        //looping among the transform's children to find the "Axes"
                        //GameObject and Activate it
                        foreach (Transform axis in raycastHitTransform)
                        {
                            //found child called Axis. Now it will be enabled
                            if (axis.name == "Axes")
                            {
                                moonAxes = axis.gameObject;
                                GameObject moon = axis.parent.Find("Moon").gameObject;
                                moonAxes.transform.rotation = Quaternion.Euler(0, 0, 0);
                                ShowHideAxes(moonAxes);
                            }
                        }
                    }
                }
                else
                {
                    ShowHideAxes(null);
                }
            }
        }



        //this method is used to decide whether the axes should be shown or hidden
        //if their are active at click time, the method hides them, otherwise it shows
        //them. If Game Mode is on Manual and the method below is called, it hides 
        //the active axes on GameObjects
        //Activating and Deactivating the Axes
        public void ShowHideAxes(GameObject axes = null)
        {
            if (prev_axes != axes)
            {
                if (axes)
                {
                    //enabling the axes only of the Game Mode is Manual
                    if (SimulationMode.IsGameModeManual())
                    {
                        if (!prev_axes)
                        {
                            axes.SetActive(true);
                        }
                        else
                        {
                            axes.SetActive(true);
                            prev_axes.SetActive(false);
                            prev_axes = null;
                        }
                    }
                }
                else
                {
                    //hiding the axes
                    if (prev_axes)
                    {
                        prev_axes.SetActive(false);
                        prev_axes = null;
                    }
                }
            }

            prev_axes = axes;
        }



        //this method simply hides all active axes. It's called from inside the 
        //inspector under the Simulation Mode Toggle's OnClick Event and also
        //in the Inspector under the Rotation or Movement Button's OnClick event
        public void HideAxes()
        {
            ShowHideAxes(null);
        }
    }
}
