using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class SimulationModeToggleController : MonoBehaviour
    {
        public void DisableSimulationModeToggle()
        {
            var toggle = gameObject.GetComponent<Toggle>();
            toggle.enabled = false;
        }

        public void EnableSimulationModeToggle()
        {
            var toggle = gameObject.GetComponent<Toggle>();
            toggle.enabled = true;
        }

        public void DisableSimulationModeToggleTemporarily()
        {
            DisableSimulationModeToggle();

            //if the objects are being reset in space then the Simulation
            //Mode toggle should not be enabled because it will be enabled
            //after the lerping operations finish anyway.
            //if the condition below is removed, it will cause bugs in the game
            if (!ResetSolarSystem.shouldResetSpaceToLastSavedPositions)
            {
                Invoke("EnableSimulationModeToggle", 2f);
            }
        }
    }
}
