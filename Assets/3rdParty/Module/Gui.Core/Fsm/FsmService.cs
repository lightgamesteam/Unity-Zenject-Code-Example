using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Gui.Core.Fsm {
    public class FsmService {
        public event Action<Type> StateChanged;
        public Type PrevStateType { get; private set; }
        
        private readonly DiContainer _container;
        private FsmStateBase _state;
        
        public FsmService(DiContainer container) {
            Assert.IsNotNull(container);
            _container = container;
        }

        public void RegisterStates<T>() {
            var stateTypeListAll = GetSubclassListThroughHierarchy<FsmStateBase>(false);
            var stateTypeList = GetTypeListWithInterface<T>(stateTypeListAll);
            foreach (var stateType in stateTypeList) {
                _container.Bind(stateType).AsTransient();
                this.Log(stateType, "registered", Color.green, Color.cyan, Color.yellow);
            }
        }

        public void SetState<T, TInfo>(TInfo info) where T : FsmState<TInfo> where TInfo : FsmInfo {
            ChangeState<T>().EnterState(info);
        }

        public void SetState<T>() where T : FsmState {
            ChangeState<T>().EnterState();
        }
        
        private T ChangeState<T>() where T : FsmStateBase {
            if (_state != null) {
                _state.ExitState();
                PrevStateType = _state.GetType();
            }
            var newState = GetState<T>();
            _state = newState;
            StateChanged?.Invoke(_state.GetType());
            return newState;
        }
        
        private T GetState<T>() where T : FsmStateBase {
            return _container.Resolve<T>();
        }
        
        #region Reflection
        
        private static readonly Type[] _allTypes = Assembly.GetExecutingAssembly().GetTypes();
        
        private static Type[] GetTypeListWithInterface<T>(IEnumerable<Type> types){
            return (from type in types let interfaceTypes = type.GetInterfaces() where interfaceTypes.Any(interfaceType => interfaceType == typeof(T)) select type).ToArray();
        }
        
        private static Type[] GetSubclassListThroughHierarchy<T>(bool inclusiveAbstract = true) {
            return _allTypes.Where(type => (inclusiveAbstract || !type.IsAbstract) && IsSubclass(typeof(T), type)).ToArray();
        }
        
        private static bool IsSubclass(Type baseType, Type subclassType) {
            if (baseType == null) { throw new ArgumentNullException(nameof(baseType)); }
            if (subclassType == null) { throw new ArgumentNullException(nameof(subclassType)); }
            
            var typeToCheck = subclassType.BaseType;
            while (typeToCheck != null) {
                if (typeToCheck == baseType) {
                    return true;
                }
                typeToCheck = typeToCheck.BaseType;
            }
            return false;
        }
        
        
        
        #endregion
    }
}