using System;
using System.Reflection;
using Module.Core.Interfaces;
using Module.Core.UIComponent;
using UnityEngine;

namespace Module.Core.Content {
    public abstract class ControllerBase : IInitializable, IReleasable {
        protected virtual void Initialize() {}
        protected virtual void Release() {}

        void IInitializable.Initialize() { Initialize(); }
        void IReleasable.Release() { Release(); }

        protected void AddDebuggerToButtons<TView>(TView view) {
            var type = view.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                var info = fieldInfo;
                var componentButton = fieldInfo.GetValue(view) as ComponentButton;
                componentButton?.AddListener(() => DebugLog("onClick", info.Name));
                var componentButtonTriggers = fieldInfo.GetValue(view) as ComponentButtonTriggers;
                componentButtonTriggers?.AddPointerDownListener(() => DebugLog("onClickTriggers", info.Name));
                var componentToggle = fieldInfo.GetValue(view) as ComponentToggle;
                componentToggle?.AddListener(value => DebugLog("onToggle:" + value, info.Name));
            }
        }

        protected void RemoveAllListeners<TView>(TView view) {
            var type = view.GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                var componentButton = fieldInfo.GetValue(view) as ComponentButton;
                componentButton?.RemoveAllListeners();
                var componentButtonTriggers = fieldInfo.GetValue(view) as ComponentButtonTriggers;
                componentButtonTriggers?.RemovePointerDownAllListeners();
                var componentToggle = fieldInfo.GetValue(view) as ComponentToggle;
                componentToggle?.RemoveAllListeners();
            }
        }

        private void DebugLog(string title, string infoName) {
            this.Log(title, infoName, Color.gray, Color.black, Color.gray);
        }
    }
}
