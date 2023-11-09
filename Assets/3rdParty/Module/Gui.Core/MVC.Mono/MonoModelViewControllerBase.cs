using UnityEngine;

namespace Gui.Core {
    public abstract class MonoModelViewControllerBase<TModel, TView> : MonoControllerBase 
        where TModel : ModelBase
        where TView : ViewBase {
        [SerializeField] private TModel _model;
        [SerializeField] private TView _view;
        
        public TModel Model => _model;
        public TView View => _view;
        
        public override void Initialize() {
            base.Initialize();
            _model.Initialize();
            _view.Initialize();
        }

        public override void Dispose() {
            _model.Dispose();
            _view.Dispose();
            base.Dispose();
        }
    }
}