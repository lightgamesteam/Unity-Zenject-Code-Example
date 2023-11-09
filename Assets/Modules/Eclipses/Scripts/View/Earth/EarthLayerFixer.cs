using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class EarthLayerFixer : MonoBehaviour
    {
        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Earth_layer");
        }
    }
}
