using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Module.Eclipses
{
    public class UIElementTouchChecker : MonoBehaviour
    {
        //this method checks whether the click or touch on the screen CASTS a ray which is directed
        //onto a UI element on the screen. It is being used to prevent GameObjects from being
        //clickable when they are right behind a UI element(implemented in BodyMovementController script)
        public static bool IsPointerOverUIElement()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}