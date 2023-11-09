using Module.Core.UIComponent;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class GuiButtonHandlerControlElementsResetModel : GuiButtonHandler {
        [Inject] private readonly SmoothOrbitCam _smoothOrbitCam = default;
        [Inject] private readonly ComponentObjectVisibility _visibility = default;

        protected override void SendSignal() {
            CameraReset(DeviceInfo.IsMobile());
        }
        
        private void CameraReset(bool autoZoom = false, bool applyNewZoomDistance = false, bool selfTarget = false) {
            if (!_smoothOrbitCam) { return; }
            
            _smoothOrbitCam.enabled = true;
            var camPos = GameObject.Find("CameraPosition");
            if (camPos != null) {
                if (selfTarget) {
                    _smoothOrbitCam.target = camPos.transform;
                }
                _smoothOrbitCam.transform.position = camPos.transform.position;
                _smoothOrbitCam.transform.LookAt(_smoothOrbitCam.target.transform);
                _smoothOrbitCam.SetDefaultValue(Vector3.Distance(_smoothOrbitCam.transform.position, _smoothOrbitCam.target.transform.position),
                    _visibility.transform.position, _smoothOrbitCam.target.transform.eulerAngles);
            } else {
                _smoothOrbitCam.transform.position = _visibility.transform.position;
                _smoothOrbitCam.transform.LookAt(_smoothOrbitCam.target.transform);
                _smoothOrbitCam.SetDefaultValue(8f, Vector3.zero, Vector3.zero);
            }
            _smoothOrbitCam.ResetMainValues();
            
            if (autoZoom || DeviceInfo.IsTablet() && DeviceInfo.IsScreenPortrait()) {
                _smoothOrbitCam.AutoZoomOnTarget(applyNewZoomDistance);
            }
        }
    }
}