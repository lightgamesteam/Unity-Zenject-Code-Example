using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class LightController : MonoBehaviour
    {
        //references the GameObject at which the light will be looking
        [SerializeField] private GameObject objectToLight;

        void Update()
        {
            //the light looks at its target
            transform.LookAt(objectToLight.transform);
        }
    }
}
