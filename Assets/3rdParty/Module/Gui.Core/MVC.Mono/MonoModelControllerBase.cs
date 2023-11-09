using UnityEngine;

namespace Gui.Core {
    public abstract class MonoModelControllerBase<TModel> : MonoControllerBase 
        where TModel : ModelBase, new() {
        [SerializeField] private TModel _model;
        
        public TModel Model => _model;
        
        public override void Initialize() {
            base.Initialize();
            _model.Initialize();
        }

        public override void Dispose() {
            _model.Dispose();
            base.Dispose();
        }
    }
}