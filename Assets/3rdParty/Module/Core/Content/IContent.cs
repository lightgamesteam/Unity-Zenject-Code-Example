namespace Module.Core.Content {
    public interface IContentModelConnectable<in TModel> { void Connect(TModel model); }
    public interface IContentViewConnectable<in TView> { void Connect(TView view); }
}