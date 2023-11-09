using System;
using Module.Core.Component;
using UnityEngine;

namespace Module.Core {
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    public class ScreenOrientationController : ControllerBase {
        #region Variables

        protected DeviceOrientation LastScreenOrientation = DeviceOrientation.Unknown;
        protected int LastScreenWidth;
        protected int LastScreenHeight;

        private event Action LandscapeEvent;
        private event Action PortraitEvent;

        #endregion

        public static ScreenOrientationController Instance {
            get {
                if (_instance == null) {
                    _instance = (ScreenOrientationController) FindObjectOfType(typeof(ScreenOrientationController));
                }
                return _instance;
            }
        }

        private static ScreenOrientationController _instance;

        #region Unity methods

        private void Awake() { OnValidateScreenOrientation(); }
        private void OnEnable() { OnValidateScreenOrientation(); }
        private void OnRectTransformDimensionsChange() { OnValidateScreenOrientation(); }

        #endregion

        #region Public methods

        public void AddScreenOrientationChange(params IScreenOrientationChange[] screenOrientationChanges) {
            foreach (var screenOrientationChange in screenOrientationChanges) {
                AddLandscapeListener(screenOrientationChange.ScreenOrientationToLandscape);
                AddPortraitListener(screenOrientationChange.ScreenOrientationToPortrait);
            }
            RecalculateScreenOrientation();
        }

        public void RemoveScreenOrientationChange(params IScreenOrientationChange[] screenOrientationChanges) {
            foreach (var screenOrientationChange in screenOrientationChanges) {
                RemoveLandscapeListener(screenOrientationChange.ScreenOrientationToLandscape);
                RemovePortraitListener(screenOrientationChange.ScreenOrientationToPortrait);
            }
        }

        public void AddLandscapeListener(Action action) { LandscapeEvent += action; }
        public void RemoveLandscapeListener(Action action) { LandscapeEvent -= action; }
        public void AddPortraitListener(Action action) { PortraitEvent += action; }
        public void RemovePortraitListener(Action action) { PortraitEvent -= action; }

        public void RecalculateScreenOrientation() {
            if (Screen.width < Screen.height) {
                LastScreenOrientation = DeviceOrientation.Portrait;
                LastScreenWidth = Screen.width;
                LastScreenHeight = Screen.height;
                ChangeScreenOrientation(LastScreenOrientation);
            } else if (Screen.width > Screen.height) {
                LastScreenOrientation = DeviceOrientation.LandscapeLeft;
                LastScreenWidth = Screen.width;
                LastScreenHeight = Screen.height;
                ChangeScreenOrientation(LastScreenOrientation);
            }
        }

        public void OnValidateScreenOrientation() {
            if (LastScreenWidth != Screen.width || 
                LastScreenHeight != Screen.height ||
                Screen.width < Screen.height && LastScreenOrientation != DeviceOrientation.Portrait || 
                Screen.width > Screen.height && LastScreenOrientation != DeviceOrientation.LandscapeLeft) {
                RecalculateScreenOrientation();
            }
        }
        
        #endregion

        #region Private methods

        private void ResetScreenOrientation() {
            LastScreenOrientation = DeviceOrientation.Unknown;
            LastScreenWidth = 0;
            LastScreenHeight = 0;
        }

        private void ChangeScreenOrientation(DeviceOrientation orientation) {
            switch (orientation) {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    PortraitEvent?.Invoke();
                    break;
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    LandscapeEvent?.Invoke();
                    break;
            }
        }

        #endregion
    }
}
