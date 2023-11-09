using System;
using System.Collections.Generic;
using Module.Core.Component;
using Module.Core.Interfaces;
using UnityEngine;

namespace Module.Core {
    public abstract class SceneControllerBase : MonoBehaviour, IInitializable, IReleasable {
        private readonly Dictionary<Type, ControllerBase> _controllers = new Dictionary<Type, ControllerBase>();

        #region Unity methods

        private void Start() {
            ConnectControllers();
            InitializeControllers();

            Initialize();
            GC.Collect(0);
        }

        private void OnDestroy() {
            foreach (var controller in _controllers) {
                (controller.Value as IReleasable).Release();
            }
            Release();
            GC.Collect();
        }

        #endregion

        #region IInitializable & IReleasable

        public virtual void Initialize() {
            this.LogGreen(gameObject.scene.name, "Initialize");
        }

        public virtual void Release() {
            this.LogGreen(gameObject.scene.name, "Release");
        }

        #endregion

        #region Connect & Initialize controllers

        private void ConnectControllers() {
            if (FindObjectsOfType(typeof(ControllerBase)) is ControllerBase[] controllers) {
                foreach (var controller in controllers) {
                    _controllers.Add(controller.GetType(), controller);
                }
            }
        }

        private void InitializeControllers() {
            foreach (var controller in _controllers) {
                try {
                    (controller.Value as IInitializable).Initialize();
                } catch (Exception ex) {
                    Debug.LogError(ex);
                    Debug.LogError(controller);
                }
            }
        }

        #endregion

        #region Methods for working with controllers

        public IScreenOrientationChange[] GetScreenOrientationChanges() {
            var result = new List<IScreenOrientationChange>();
            foreach (var controllerBase in _controllers) {
                if (controllerBase.Value is IScreenOrientationChange change) {
                    result.Add(change);
                }
            }
            return result.ToArray();
        }

        public T Get<T>() where T : ControllerBase {
            var type = typeof(T);
            if (_controllers.ContainsKey(type)) {
                return _controllers[type] as T;
            }
            return default(T);
        }

        public bool TryGet<T>(out T controller) where T : ControllerBase {
            controller = Get<T>();
            return controller != null;
        }

        #endregion
    }
}
