using System.Collections;
using UnityEngine;

namespace Module.DeveloperDebugging.Core {
    public abstract class ComponentBase<TView, TController> : ControllerBase 
        where TController: ComponentControllerBase<TView>, new() {
        [SerializeField] private TView _view;
        [SerializeField] private TController _controller;

        #region Variables

        public TView View => _view;
        public TController Controller => _controller;

        protected Coroutine UpdateCoroutine;

        #endregion

        #region Public methods

        public override void Initialize() {
            base.Initialize();
            _controller = new TController();
            ((IComponentViewConnectable<TView>)_controller).Connect(_view);
            ((IInitializable)_controller).Initialize();
        }

        public override void Release() {
            ((IReleasable) _controller)?.Release();
            base.Release();
        }

        public virtual void Activate(MonoBehaviour monoBehaviour, IEnumerator enumerator = null) {
            if (enumerator != null) {
                UpdateCoroutine = monoBehaviour.StartCoroutine(enumerator);
            }
        }

        public virtual void Deactivate(MonoBehaviour monoBehaviour) {
            if (UpdateCoroutine != null) {
                monoBehaviour.StopCoroutine(UpdateCoroutine);
            }
        }

        #endregion
    }
}