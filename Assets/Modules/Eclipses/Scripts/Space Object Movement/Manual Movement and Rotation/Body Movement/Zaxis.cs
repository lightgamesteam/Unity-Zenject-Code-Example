using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    //this script helps move the Sun, the Moon and Earth along their Z axis when the 
    //movement Axes are enabled and visible on them and when the game is on Manual Mode

    public class Zaxis : MonoBehaviour
    {

        // Get world position objects
        private Vector3 screenPoint;
        // Difference object vector and input
        private Vector3 offset;

        private bool canChangePos = true;

        void OnMouseDown()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            //should work only on Manual Mode and when the touch or click on the screen do not target a UI element
            if (SimulationMode.IsGameModeManual() && !UIElementTouchChecker.IsPointerOverUIElement() && Input.GetMouseButtonDown(0))
            {
                screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.parent.parent.position);
                offset = gameObject.transform.parent.parent.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            }
#elif UNITY_IOS || UNITY_ANDROID
        //should work only on Manual Mode and when the touch or click on the screen do not target a UI element
        if (SimulationMode.IsGameModeManual() && !UIElementTouchChecker.IsPointerOverUIElement() && Input.touchCount == 1)
        {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.parent.parent.position);
            offset = gameObject.transform.parent.parent.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
#endif
        }
        private void OnMouseUp()
        {
            canChangePos = true;
        }

        //change the objects position depending on the axes' movement 
        void OnMouseDrag()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            //should work only on Manual Mode and when the touch or click on the screen do not target a UI element
            if (SimulationMode.IsGameModeManual() && !UIElementTouchChecker.IsPointerOverUIElement() && canChangePos && Input.GetMouseButton(0))
            {
                Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
                transform.parent.parent.position = new Vector3(transform.parent.parent.position.x, transform.parent.parent.position.y, cursorPosition.z);
            }
            else
            {
                canChangePos = false;
            }
#elif UNITY_IOS || UNITY_ANDROID
        //should work only on Manual Mode and when the touch or click on the screen do not target a UI element
        if (SimulationMode.IsGameModeManual() && !UIElementTouchChecker.IsPointerOverUIElement() && canChangePos && Input.touchCount == 1)
        {
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            transform.parent.parent.position = new Vector3(transform.parent.parent.position.x, transform.parent.parent.position.y, cursorPosition.z);
        }
        else
        {
            canChangePos = false;
        }
#endif
        }
    }
}