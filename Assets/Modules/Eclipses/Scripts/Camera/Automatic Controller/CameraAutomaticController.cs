using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class CameraAutomaticController : MonoBehaviour
    {
        //referencing the GameObjects which will become parents
        [SerializeField] private GameObject _solarSystem;
        [SerializeField] private GameObject _earthAndMoon;
        [SerializeField] private GameObject _moon;
        [SerializeField] private GameObject _pin;

        //these positions and rotations are set up in the inspector under the MainCamera GameObject
        //they are used to switch the Camera's position between 4 predefined views
        [SerializeField] private Vector3 viewFromDistancePosition;
        [SerializeField] private Vector3 viewFromDistanceRotation;
        [SerializeField] private Vector3 viewFromDistancePosition_2;
        [SerializeField] private Vector3 viewFromDistanceRotation_2;
        [SerializeField] private Vector3 viewFromEarthPosition;
        [SerializeField] private Vector3 viewFromEarthRotation;
        [SerializeField] private Vector3 viewFromSunPosition;
        [SerializeField] private Vector3 viewFromSunRotation;
        [SerializeField] private Vector3 closeViewAtEarthPosition;
        [SerializeField] private Vector3 closeViewAtEarthRotation;
        [SerializeField] private Vector3 viewFromPinOnMoonPosition;
        [SerializeField] private Vector3 viewFromPinOnMoonRotation;
        [SerializeField] private Vector3 viewFromMoonOnPinPosition;
        [SerializeField] private Vector3 viewFromMoonOnPinRotation;

        public bool LunarEclipses;

        //initializing with the main view which will be used to set up the camera at launch time
        [HideInInspector] public CameraPosition cameraPosition = CameraPosition.viewFromDistance;

        private void Awake()
        {
            //setting the Camera's position to the view from distance at 
            //start time(this is the initial position of the camera when the game starts)
            ChangeCameraPosition(cameraPos: cameraPosition);
        }

        //this method changes the Camera's position between 4 predefined views
        //this method is called in the Awake function of this script, in the
        // Simulation Mode switcher method in the SimulationMode script
        //and in the SwitchCameraPosition method in this script
        public void ChangeCameraPosition(CameraPosition cameraPos)
        {
            switch (cameraPos)
            {
                //the default position of the camera
                case CameraPosition.viewFromDistance:
                    //making the camera Solar System's child object so that it stays where
                    //it is and does not rotate with the moon and Earth
                    SetCameraParent(newParent: _solarSystem);
                    //setting the Camera's corresponding Position and Rotation values
                    transform.position = viewFromDistancePosition;
                    transform.eulerAngles = viewFromDistanceRotation;
                    break;
                case CameraPosition.viewFromDistance_2:
                    //setting the Camera's corresponding Position and Rotation values
                    transform.position = viewFromDistancePosition_2;
                    transform.eulerAngles = viewFromDistanceRotation_2;
                    break;
                case CameraPosition.viewFromEarth:
                    //making the camera EarthAndMoon's child object so that it constantly
                    //rotates with the Moon and Earth and looks at them
                    SetCameraParent(newParent: _earthAndMoon);
                    //setting the Camera's corresponding Position and Rotation values
                    transform.localPosition = viewFromEarthPosition;
                    transform.localEulerAngles = viewFromEarthRotation;
                    break;
                case CameraPosition.viewFromSun:
                    //setting the Camera's corresponding Position and Rotation values
                    transform.localPosition = viewFromSunPosition;
                    transform.localEulerAngles = viewFromSunRotation;
                    break;
                case CameraPosition.closeViewAtEarth:
                    //setting the Camera's corresponding Position and Rotation values
                    transform.localPosition = closeViewAtEarthPosition;
                    transform.localEulerAngles = closeViewAtEarthRotation;
                    break;
                case CameraPosition.viewFromPin:
                    transform.localPosition = viewFromPinOnMoonPosition;
                    transform.LookAt(_moon.transform);
                    break;
                case CameraPosition.viewFromMoon:
                    SetCameraParent(newParent: _moon.transform.parent.gameObject);
                    transform.localPosition = new Vector3(0, 0, 0);
                    transform.LookAt(_pin.transform);
                    break;
                default:
                    //making the camera Solar System's child object so that it stays where
                    //it is and does not rotate with the moon and Earth
                    SetCameraParent(newParent: _solarSystem);
                    //setting the Camera's corresponding Position and Rotation values
                    transform.position = viewFromDistancePosition;
                    transform.eulerAngles = viewFromDistanceRotation;
                    break;
            }
        }

        //this method is called when the Camera position change button is clicked on
        //main screen, every time the button is clicked on the position switches
        //to the next one in order
        public void SwitchCameraPosition()
        {
            switch (cameraPosition)
            {
                case CameraPosition.viewFromDistance:
                    cameraPosition = CameraPosition.viewFromDistance_2;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                case CameraPosition.viewFromDistance_2:
                    cameraPosition = CameraPosition.viewFromEarth;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                case CameraPosition.viewFromEarth:
                    cameraPosition = CameraPosition.viewFromSun;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                case CameraPosition.viewFromSun:
                    cameraPosition = CameraPosition.closeViewAtEarth;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                case CameraPosition.closeViewAtEarth:
                    if (LunarEclipses)
                    {
                        cameraPosition = CameraPosition.viewFromPin;
                    }
                    else
                    {
                        cameraPosition = CameraPosition.viewFromDistance;
                    }
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                case CameraPosition.viewFromPin:
                    cameraPosition = CameraPosition.viewFromMoon;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                case CameraPosition.viewFromMoon:
                    cameraPosition = CameraPosition.viewFromDistance;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
                default:
                    cameraPosition = CameraPosition.viewFromDistance;
                    ChangeCameraPosition(cameraPos: cameraPosition);
                    break;
            }
        }

        //this method makes the camera the child of 
        //a GameObject with a tag passed to it as a parameter
        public void SetCameraParent(GameObject newParent)
        {
            //making the Camera the newParent's child
            transform.parent = newParent.transform;
        }

        //there will be 4 different positions of the camera
        //when the Simulation Mode is Automatic
        public enum CameraPosition
        {
            //the inital and main position of the Camera(sees the whole solar system)
            viewFromDistance,
            //a position from which one can see that Earth's orbit around the Sun is elliptical
            viewFromDistance_2,
            //camera looks at Earth, the Moon and the Sun at a close distance
            viewFromEarth,
            //camera looks at the Sun, Earth, and the Moon
            viewFromSun,
            //camera looks at the Moon and Earth from a close distance
            closeViewAtEarth,
            viewFromPin,
            viewFromMoon,
        }
        public void ChangeBoolEclipseForFalse ()
        {
            LunarEclipses = false;
        }
    }
}
