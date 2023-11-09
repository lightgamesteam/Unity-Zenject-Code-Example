using Module.DeveloperDebugging.Components.Console;
using Module.DeveloperDebugging.Components.PerformanceData;
using UnityEngine;

namespace Module.DeveloperDebugging {
    public class DD : MonoBehaviour {
        #region Inspector

        [SerializeField] protected ComponentPerformanceData ComponentPerformanceData;
        [SerializeField] protected ComponentConsole ComponentConsole;
        [SerializeField] protected bool KeepAlive = true;

        #endregion

        #region Variables

        protected static DD Instance { get; private set; }

        private bool _initialized;

        #endregion

        #region Unity methods

        private void Awake() {
            if (Instance != null && Instance.KeepAlive) {
                Destroy(this);
                return;
            }

            Instance = this;

            ComponentsInitialize();
            _initialized = true;
        }

        private void Start() {
            if (KeepAlive) {
                DontDestroyOnLoad(transform.root.gameObject);
            }
        }

        private void OnDestroy() {
            if (!_initialized) { return; }

            ComponentsRelease();
        }

        private void OnEnable() {
            if(!_initialized) { return; }

            ComponentPerformanceData.Activate(this);
        }

        private void OnDisable() {
            if (!_initialized) { return; }

            ComponentPerformanceData.Deactivate(this);
        }

        #endregion

        #region Public methods

        public static class Console {
            public static void Clear() {
                Instance?.ComponentConsole.Controller.Clear();
            }

            public static void Log(string value) {
                Instance?.ComponentConsole.Controller.Log(value);
            }
        }

        #endregion

        #region Private methods

        private void ComponentsInitialize() {
            ComponentPerformanceData.Initialize();
            ComponentConsole.Initialize();
        }

        private void ComponentsRelease() {
            ComponentPerformanceData.Release();
            ComponentConsole.Release();
        }

        #endregion
    }
}
