namespace Module.DeveloperDebugging.Core {
    public interface IInitializable {
        void Initialize();
    }

    public interface IReleasable {
        void Release();
    }

    public interface IComponentViewConnectable<in TView> {
        void Connect(TView view);
    }
}
