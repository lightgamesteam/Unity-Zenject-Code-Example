using UnityEngine;
using System;
using System.Collections.Generic;

namespace Module.PeriodicTable
{
    [Serializable]
    public class Description
    {
        public string Name;
        public string Value;
    }

    [Serializable]
    public class PropertiesPeriodicElement
    {
        public List<Description> AtomicProperties;
        public List<Description> PhysicalProperties;
    }


    public class PeriodicModel : MonoBehaviour
    {

        [Serializable]
        public struct PeriodicElement
        {
            [HideInInspector] public string name;
            [HideInInspector] public string type;

            [HideInInspector] [TextArea] public string description_en;
            [HideInInspector] [TextArea] public string description_no;

            public GameObject gameObject;
            [HideInInspector] public Vector3 position;
            [HideInInspector] public Material material;
        }

        [Serializable]
        public struct PeriodicType
        {
            public string name;
            public GameObject gameObject;
            public GameObject button;
            [HideInInspector] public Vector3 positionButton;
            public List<PeriodicElement> periodicElement;
        }

        public static List<PeriodicType> PeriodicTypes = new List<PeriodicType>();
        public static List<PeriodicElement> PeriodicElements = new List<PeriodicElement>();
        public static PropertiesPeriodicElement CurrentDescription = new PropertiesPeriodicElement();
    }
}