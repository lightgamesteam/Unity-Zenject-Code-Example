using System;
using TDL.Constants;
using Zenject;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDL.Views
{
    public class ScreenOrientationMediator : IInitializable, IDisposable
    {
        private ScreenOrientation _lastScreenOrientation;
        private ScreenOrientation _currentScreenOrientation;

        public void Initialize()
        { 
            Screen.orientation = ScreenOrientation.Portrait;
            RotateScreen(ScreenOrientation.Portrait);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(SceneNameConstants.Module2DVideo) || scene.name.Equals(SceneNameConstants.Module3DVideo))
            {
                _lastScreenOrientation = _currentScreenOrientation;
                RotateScreen(ScreenOrientation.LandscapeLeft);
            }
            else
            {
                RotateScreen(ScreenOrientation.Portrait);
            }
        }
    
        private void OnSceneUnloaded(Scene scene)
        {
            if(scene.name.Equals(SceneNameConstants.Module2DVideo) || scene.name.Equals(SceneNameConstants.Module3DVideo))
            {
                RotateScreen(_lastScreenOrientation);
            }
            else
            {
                RotateScreen(ScreenOrientation.Portrait);
            }
        }

        private void RotateScreen(ScreenOrientation screenOrientation)
        {
            _currentScreenOrientation = screenOrientation;
            Screen.orientation = screenOrientation;
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }
   
}