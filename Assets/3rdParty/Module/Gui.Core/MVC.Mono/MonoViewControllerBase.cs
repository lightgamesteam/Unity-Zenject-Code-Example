using UnityEngine;

namespace Gui.Core {
    public abstract class MonoViewControllerBase<TView> : MonoControllerBase 
        where TView : ViewBase {
        [SerializeField] private TView _view;
        
        public TView View => _view;
        
        public override void Initialize() {
            base.Initialize();
            _view.Initialize();
        }

        public override void Dispose() {
            _view.Dispose();
            base.Dispose();
        }
        
        #region Screen orientation
        
        private DeviceOrientation _lastScreenOrientation = DeviceOrientation.Unknown;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        
        private void Awake() { OnValidateScreenOrientation(); }
        private void OnEnable() { OnValidateScreenOrientation(); }
        private void OnRectTransformDimensionsChange() { OnValidateScreenOrientation(); }
        
        protected virtual void ScreenOrientationToLandscape() {}
        protected virtual void ScreenOrientationToPortrait() {}
        
        private void OnValidateScreenOrientation() {
            if (_lastScreenWidth == Screen.width &&
                _lastScreenHeight == Screen.height &&
                (Screen.width >= Screen.height || _lastScreenOrientation == DeviceOrientation.Portrait) &&
                (Screen.width <= Screen.height || _lastScreenOrientation == DeviceOrientation.LandscapeLeft)) {
                return;
            }
            
            if (Screen.width < Screen.height) {
                _lastScreenOrientation = DeviceOrientation.Portrait;
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
                ChangeScreenOrientation(_lastScreenOrientation);
            } else if (Screen.width > Screen.height) {
                _lastScreenOrientation = DeviceOrientation.LandscapeLeft;
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
                ChangeScreenOrientation(_lastScreenOrientation);
            }
        }

        private void ChangeScreenOrientation(DeviceOrientation orientation) {
            switch (orientation) {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    ScreenOrientationToPortrait();
                    break;
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    ScreenOrientationToLandscape();
                    break;
            }
        }
        
        #endregion
    }
}