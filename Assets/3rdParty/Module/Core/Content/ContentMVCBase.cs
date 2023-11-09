using Module.Core.Interfaces;
using UnityEngine;

namespace Module.Core.Content {
    public abstract class ContentCBase<TController> : ContentBase
        where TController : ContentControllerBase, new() {
        [SerializeField] private TController _controller;

        #region Variables

        public TController Controller => _controller;

        #endregion

        #region Public methods

        public override void Initialize() {
            base.Initialize();
            _controller = new TController();
            (_controller as IInitializable)?.Initialize();
            SetStateComponent(false);
        }

        public override void Release() {
            (_controller as IReleasable)?.Release();
            base.Release();
        }

        #endregion
    }

    public abstract class ContentMCBase<TModel, TController> : ContentBase
        where TModel : new()
        where TController : ContentControllerWithModelBase<TModel>, new() {
        [SerializeField] private TModel _model;
        [SerializeField] private TController _controller;

        #region Variables

        public TModel Model => _model;
        public TController Controller => _controller;

        #endregion

        #region Public methods

        public override void Initialize() {
            base.Initialize();
            _model = new TModel();
            _controller = new TController();
            ((IContentModelConnectable<TModel>) _controller).Connect(_model);
            (_model as IInitializable)?.Initialize();
            (_controller as IInitializable)?.Initialize();
            SetStateComponent(false);
        }

        public override void Release() {
            (_model as IReleasable)?.Release();
            (_controller as IReleasable)?.Release();
            base.Release();
        }

        #endregion
    }

    public abstract class ContentVCBase<TView, TController> : ContentBase
        where TController : ContentControllerWithViewBase<TView>, new() {
        [SerializeField] private TView _view;
        [SerializeField] private TController _controller;

        #region Variables

        public TView View => _view;
        public TController Controller => _controller;

        #endregion

        #region Public methods

        public override void Initialize() {
            base.Initialize();
            _controller = new TController();
            ((IContentViewConnectable<TView>) _controller).Connect(_view);
            (_view as IInitializable)?.Initialize();
            (_controller as IInitializable)?.Initialize();
            SetStateComponent(false);
        }

        public override void Release() {
            (_view as IReleasable)?.Release();
            (_controller as IReleasable)?.Release();
            base.Release();
        }

        #endregion
    }

    public abstract class ContentMVCBase<TModel, TView, TController> : ContentBase
        where TModel : new()
        where TController : ContentControllerWithModelViewBase<TModel, TView>, new() {
        [SerializeField] private TModel _model;
        [SerializeField] private TView _view;
        [SerializeField] private TController _controller;

        #region Variables

        public TModel Model => _model;
        public TView View => _view;
        public TController Controller => _controller;

        #endregion

        #region Public methods

        public override void Initialize() {
            base.Initialize();
            _model = new TModel();
            _controller = new TController();
            ((IContentModelConnectable<TModel>) _controller).Connect(_model);
            ((IContentViewConnectable<TView>) _controller).Connect(_view);
            (_model as IInitializable)?.Initialize();
            (_view as IInitializable)?.Initialize();
            (_controller as IInitializable)?.Initialize();
            SetStateComponent(false);
        }

        public override void Release() {
            (_model as IReleasable)?.Release();
            (_view as IReleasable)?.Release();
            (_controller as IReleasable)?.Release();
            base.Release();
        }

        #endregion
    }
}