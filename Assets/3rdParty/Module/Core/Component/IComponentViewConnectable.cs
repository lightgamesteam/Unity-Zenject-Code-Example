namespace Module.Core.Component {
    public interface IComponentViewConnectable<in TView> {
        void Connect(TView view);
    }
}