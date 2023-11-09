using UnityEngine;

namespace Module.Eclipses
{
    // Base class for all elements in this application.
    public class SpaceModuleElement : MonoBehaviour
    {
        // Gives access to the application and all instances.
        public SpaceModuleApplication app => FindObjectOfType<SpaceModuleApplication>();
    }
    
    public class SpaceModuleApplication : MonoBehaviour
    {
        // Reference to the root instances of the MVC.
        public ControllerSpaceModule controller;
        public ModelSpaceModule model;
        public ViewSpaceModule view;

        public Material CustomSkybox;
        private Material _defaultSkybox;

        private void OnEnable()
        {
            _defaultSkybox = RenderSettings.skybox;
            RenderSettings.skybox = CustomSkybox;
        }

        private void OnDisable()
        {
            RenderSettings.skybox = _defaultSkybox;
        }
    }
}