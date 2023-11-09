using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class PinDarkSideCheckerQuadController : SpaceModuleElement
    {
        //the sun
        [SerializeField] private GameObject _parentSun;
        //the pin which appears on Earth's surface when
        //a location is being chosen on its surface
        //to find eclipses
        [SerializeField] private GameObject _pin;

        //used to check whether Earth has apready reached
        //a rotation point where the eclipse location (pin) 
        //directly faces the sun(so that a shadow can be cast over it)
        private bool isEarthRotatedCorrectly = false;
        //used to check whether Earth still has to rotate to
        //reach a desired rotation point
        private bool shouldChangeRotationOfEarth = false;

        //this will trigger SetUp operations for the Mon
        //when Earth has been rotated so that the pin faces the Sun
        [SerializeField] private MoonEclipseSetup moonEclipseSetup;

        private void Update()
        {
            //if Earth still has to rotate to reach a desired rotation point
            //and if it still has not reached that desired rotation point
            if (isEarthRotatedCorrectly == false && shouldChangeRotationOfEarth == true)
            {
                //rotating Earth
                app.view._earthView.transform.Rotate(new Vector3(0, 5, 0));
                //cancelling all forces from the DarkSideChecker GameObject
                //because its RigidBody accumulates forces when Earth rotates
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //
            }
        }

        //this method makes the DarkSideChecker GameObject constantly face the Sun
        //and be positioned closer to the sun so that the code can check whether
        //the pin is inside the DarkSideChecker's collider (meaning that 
        //it has reached the desired rotation point)
        private void DarkSideCheckerLookAtSun()
        {
            //every time the method is called the DarkSideChecker is set at the center of its parent Earth
            gameObject.transform.position = gameObject.transform.parent.position;
            //the rotation variable holds the direction in which the DarkSideChecker GameObject 
            //should be rotated to face the Sun
            var rotation = Quaternion.LookRotation(_parentSun.transform.position - gameObject.transform.position);
            //setting the rotation so that the DarkSideChecker faces the Sun
            gameObject.transform.rotation = rotation;
            //this is the direction in which the DarkSideChecker GameObject should be moved
            //to come a little bit closer to the Sun
            var direction = _parentSun.transform.position - gameObject.transform.position;
            //moving the DarkSideChecker GameObject a little bit closer to the Sun
            //so that its collider is positioned exceptionally on the part of Earth closer to the Sun
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, direction, gameObject.transform.parent.localScale.x * 1.2f);
        }

        //checking if the pin on Earth's surface is on the part of
        //Earth that is positioned closer to the Sun(directly faces the Sun)
        public void CheckIfPinIsOnDarkSideWithDelay()
        {
            //checks if the pin faces the Sun
            CheckIfPinIsOnDarkSide();
            //informs the Update function to rotate Earth so that the pin reaches
            //a desired position and faces the Sun
            shouldChangeRotationOfEarth = true;
        }

        public void CheckIfPinIsOnDarkSide()
        {
            //activating the DarkSideChecker GameObject
            gameObject.SetActive(true);
            //this makes the DarkSideChecker GameObject constantly be at a 
            //position where it can correctly check whether the pin has come
            //closer to the Sun
            DarkSideCheckerLookAtSun();

            //if Earth has already rotated to its desired rotation point
            //where the pin faces the Sun and is visible when looked upon
            //from the direction of the Sun
            if (isEarthRotatedCorrectly == true)
            {
                //setting this to false so that it works correctly
                //next time this method is used when an eclipse is chosen
                isEarthRotatedCorrectly = false;
                //no longer Earth needs to be rotated
                shouldChangeRotationOfEarth = false;
                //cancelling all forces from the DarkSideChecker
                //GameObject's RigidBody so that it becomes stationary
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //
                //hiding the DarkSideChecker GameObject
                gameObject.SetActive(false);

                //setting up Moon so that its shadow falls onto the location
                //of the pin on Earth's surface
                moonEclipseSetup.SetUpMoonToSimulateEclipse();
            }
            //if the pin still is not visible when looked upon from the Sun's direction
            //and Earth still has to rotate
            else
            {
                //calling this function again so that it can continue checking everything
                Invoke("CheckIfPinIsOnDarkSide", 0.05f);
            }
        }

        //checks if the pin has entered the DarkSideChecker
        //GameObject's collider
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "pin")
            {
                //means that Earth no longer needs to be rotated
                //since the pin is already facing the Sun
                isEarthRotatedCorrectly = true;
            }
        }
    }
}
