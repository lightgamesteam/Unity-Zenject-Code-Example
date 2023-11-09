using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class EarthPositionChooserController : MonoBehaviour
    {
        //used to turn on the collider component on the parent
        //so that it does not confront other functionality(location choosing on Earth's surface)
        [SerializeField] private GameObject _parentEarth;

        //disables the Sphere Collider component on Earth so that a position
        //can be chosen on its surface since this functionality is blocked by the collider
        public void EnablePositionChoosing()
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }

        //enabling the Sphere Collider component so that Earth can be moved or Rotated
        //Location choosing on Earth's surface is hindered when this collider is enabled
        public void DisablePositionChoosing()
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }
}
