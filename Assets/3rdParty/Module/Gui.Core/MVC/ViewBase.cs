using System;
using System.Reflection;
using Module;
using Module.Core.UIComponent;
using UnityEngine;
using Zenject;

namespace Gui.Core {
    [Serializable]
    public abstract class ViewBase : IInitializable, IDisposable {
        public virtual void Initialize() {
            //AddDebuggerToButtons();
        }

        public virtual void Dispose() {
            //RemoveAllListeners();
        }

        private void AddDebuggerToButtons() {
            var type = GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                // ComponentButton
                var componentButton = fieldInfo.GetValue(this) as ComponentButton;
                componentButton?.AddListener(() => DebugLog("onClick", fieldInfo.Name));
                // ComponentButtonTriggers
                var componentButtonTriggers = fieldInfo.GetValue(this) as ComponentButtonTriggers;
                componentButtonTriggers?.AddPointerDownListener(() => DebugLog("onClickTriggers", fieldInfo.Name));
                // ComponentToggle
                var componentToggle = fieldInfo.GetValue(this) as ComponentToggle;
                componentToggle?.AddListener(value => DebugLog("onToggle:" + value, fieldInfo.Name));
            }
        }

        private void RemoveAllListeners() {
            var type = GetType();
            var fields = type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fields) {
                // ComponentButton
                var componentButton = fieldInfo.GetValue(this) as ComponentButton;
                componentButton?.RemoveAllListeners();
                // ComponentButtonTriggers
                var componentButtonTriggers = fieldInfo.GetValue(this) as ComponentButtonTriggers;
                componentButtonTriggers?.RemovePointerDownAllListeners();
                // ComponentToggle
                var componentToggle = fieldInfo.GetValue(this) as ComponentToggle;
                componentToggle?.RemoveAllListeners();
            }
        }
        
        private void DebugLog(string title, string infoName) {
            this.Log(title, infoName, Color.gray, Color.black, Color.gray);
        }
    }
}