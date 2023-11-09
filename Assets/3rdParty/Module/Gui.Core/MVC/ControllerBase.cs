using System;
using Module;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Gui.Core {
    [Serializable]
    public abstract class ControllerBase : MonoBehaviour, IInitializable, IDisposable {
        protected DiContainer Container;
        
        public virtual void Initialize() {
            //this.Log("Initialize", Color.green, Color.yellow);
        }

        public virtual void Dispose() {
            //this.Log("Dispose", Color.green, Color.red);
        }
        
        [Inject]
        private void Construct(DiContainer container) {
            Container = container;
            Initialize();
        }

        protected T InstantiatePrefab<T>(GameObject prefab) {
            return InstantiatePrefab<T>(prefab, transform);
        }

        protected T InstantiatePrefab<T>(GameObject prefab, Transform parent) {
            Assert.IsNotNull(prefab);
            var go = Container.InstantiatePrefab(prefab);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go.GetComponent<T>();
        }
    }
}