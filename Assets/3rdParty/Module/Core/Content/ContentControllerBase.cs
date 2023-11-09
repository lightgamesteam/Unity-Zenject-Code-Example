namespace Module.Core.Content {
    public abstract class ContentControllerBase : ControllerBase {}

    public abstract class ContentControllerWithModelBase<TModel> : ControllerBase,
        IContentModelConnectable<TModel> {
        public TModel Model;

        void IContentModelConnectable<TModel>.Connect(TModel model) { Model = model; }
    }

    public abstract class ContentControllerWithViewBase<TView> : ControllerBase,
        IContentViewConnectable<TView> {
        public TView View;

        void IContentViewConnectable<TView>.Connect(TView view) { View = view; }

        protected override void Initialize() { AddDebuggerToButtons(View); }
        protected override void Release() { RemoveAllListeners(View); }
    }

    public abstract class ContentControllerWithModelViewBase<TModel, TView> : ControllerBase,
        IContentModelConnectable<TModel>, IContentViewConnectable<TView> {
        public TModel Model;
        public TView View;

        void IContentModelConnectable<TModel>.Connect(TModel model) { Model = model; }
        void IContentViewConnectable<TView>.Connect(TView view) { View = view; }

        protected override void Initialize() { AddDebuggerToButtons(View); }
        protected override void Release() { RemoveAllListeners(View); }
    }
}