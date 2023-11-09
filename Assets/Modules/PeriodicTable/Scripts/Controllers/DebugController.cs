using System.Collections.Generic;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class DebugController : MonoBehaviour
    {

        public static void DebugInfo(List<PeriodicModel.PeriodicType> _periodicTypes)
        {
            if (_periodicTypes.Count > 0)
            {
                Debug.Log("<color=green>Success!</color> \nThe total number of button types <b>" +
                          _periodicTypes.Count +
                          "</b>");
            }
            else
            {
                Debug.LogError("<color=red>Error!</color> \nNo items found...");
            }
        }

        public static void DebugInfo(List<PeriodicModel.PeriodicElement> periodicElements)
        {
            if (periodicElements.Count > 0)
            {
                Debug.Log("<color=green>Success!</color> \nThe total number of periodic elements <b>" +
                          periodicElements.Count +
                          "</b>;  <i>Number of periodic types - <b>" + PeriodicModel.PeriodicTypes.Count + "</b></i>");
            }
            else
            {
                Debug.LogError("<color=red>Error!</color> \nNo items found...");
            }
        }

    }
}