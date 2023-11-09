using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Core {
    public class ModuleControllerBase : SceneControllerBase {
        private StateControllerBase _currentState;

        public static ModuleControllerBase Instance => ConnectInstance(ref _instance);

        private static ModuleControllerBase _instance;
        
        private readonly Dictionary<Camera, bool> _defaultCamerasOutOfScene = new Dictionary<Camera, bool>();

        public static T ConnectInstance<T>(ref T instance) where T : SceneControllerBase {
            if (instance == null) {
                instance = (T) FindObjectOfType(typeof(T));
                if (FindObjectsOfType(typeof(T)).Length > 1) {
                    Debug.LogWarning("[Singleton] Something went really wrong " +
                                     " - there should never be more than 1 singleton!" +
                                     " Reopening the scene might fix it.");
                    return instance;
                }
                if (instance == null) {
                    var obj = new GameObject(typeof(T) + "_Singleton");
                    instance = obj.AddComponent<T>();
                    Debug.Log("Create singleton: " + typeof(T));
                }
            }
            return instance;
        }

        public override void Initialize() {
            base.Initialize();
            DisableAllCamerasOutOfScene();
        }

        public override void Release() {
            ResetAllCamerasOutOfScene();
            base.Release();
        }

        public void ActiveState<T>() where T : StateControllerBase {
            var state = Get<T>();
            if (state == null) {
                this.LogErrorRed(typeof(T), "This state does not exist.");
            }

            ChangeState(state);
        }

        private void ChangeState(StateControllerBase state) {
            if (_currentState != null) {
                _currentState.ExitState();
            }
            if (state != null) {
                _currentState = state;
                _currentState.EnterState();
                ScreenOrientationController.Instance?.RecalculateScreenOrientation();
            }
        }

        private void DisableAllCamerasOutOfScene() {
            var allCameras = FindObjectsOfType<Camera>();
            foreach (var camera in allCameras) {
                if (!camera.gameObject.scene.Equals(gameObject.scene)) {
                    _defaultCamerasOutOfScene.Add(camera, camera.enabled);
                    camera.enabled = false;
                }
            }
        }
        
        private void ResetAllCamerasOutOfScene() {
            foreach (var camera in _defaultCamerasOutOfScene.Keys) {
                if (camera != null && camera.gameObject != null) {
                    camera.enabled = _defaultCamerasOutOfScene[camera];
                }
            }
        }
    }
}
