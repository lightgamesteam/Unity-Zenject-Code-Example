using System.Reflection;
using Module.Core.Content;
using Module.Core.UIComponent;
using Module.Core.Interfaces;
using UnityEngine;

namespace Module.Core.Component {
    public abstract class ComponentControllerBase<TView> : IComponentViewConnectable<TView>, IInitializable, IReleasable {
        #region Variables

        public TView View;

        #endregion

        #region Interface

        void IComponentViewConnectable<TView>.Connect(TView view) { View = view; }

        void IInitializable.Initialize() { Initialize(); }

        void IReleasable.Release() { Release(); }

        #endregion

        #region Protected methods

        protected virtual void Initialize() {
            AddDebuggerToButtons();
            InitializeContents();
        }

        protected virtual void Release() {
            RemoveAllListeners();
            ReleaseContents();
        }

        protected void DebugLog(string title, string infoName) {
            this.Log(title, infoName, Color.gray, Color.black, Color.gray);
        }

        #endregion

        #region Private methods

        private void InitializeContents() {
            var type = View.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                var monoControllerBase = fieldInfo.GetValue(View) as ContentBase;
                monoControllerBase?.Initialize();
            }
        }

        private void ReleaseContents() {
            var type = View.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                var monoControllerBase = fieldInfo.GetValue(View) as ContentBase;
                monoControllerBase?.Release();
            }
        }

        private void AddDebuggerToButtons() {
            var type = View.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                var info = fieldInfo;
                var componentButton = fieldInfo.GetValue(View) as ComponentButton;
                componentButton?.AddListener(() => DebugLog("onClick", info.Name));
                var componentButtonTriggers = fieldInfo.GetValue(View) as ComponentButtonTriggers;
                componentButtonTriggers?.AddPointerDownListener(() => DebugLog("onClickTriggers", info.Name));
                var componentToggle = fieldInfo.GetValue(View) as ComponentToggle;
                componentToggle?.AddListener(value => DebugLog("onToggle:" + value, info.Name));
            }
        }

        private void RemoveAllListeners() {
            var type = View.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                var componentButton = fieldInfo.GetValue(View) as ComponentButton;
                componentButton?.RemoveAllListeners();
                var componentButtonTriggers = fieldInfo.GetValue(View) as ComponentButtonTriggers;
                componentButtonTriggers?.RemovePointerDownAllListeners();
                var componentToggle = fieldInfo.GetValue(View) as ComponentToggle;
                componentToggle?.RemoveAllListeners();
            }
        }

        #endregion
    }
}