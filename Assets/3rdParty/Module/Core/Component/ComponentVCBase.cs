using Module.Core.Interfaces;
using UnityEngine;

namespace Module.Core.Component {
    public abstract class ComponentVCBase<TView, TController> : ComponentBase 
        where TController: ComponentControllerBase<TView>, new() {
        #region Inspector

        [SerializeField] private TView _view;
        [SerializeField] private TController _controller;

        #endregion

        #region Variables

        public TView View => _view;
        public TController Controller => _controller;

        #endregion

        #region Protected methods

        protected override void Initialize() {
            base.Initialize();
            _controller = new TController();
            ((IComponentViewConnectable<TView>)_controller).Connect(_view);
            ((IInitializable)_controller).Initialize();
            HideComponent();
        }

        protected override void Release() {
            ((IReleasable) _controller)?.Release();
            base.Release();
        }

        #endregion
    }
}