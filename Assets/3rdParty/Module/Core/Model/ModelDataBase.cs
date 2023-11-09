namespace Module.Core.Model {
    public abstract class ModelDataBase {
        public virtual string PurposeName { get; protected set; }
        public virtual string FileHash { get; protected set; }
        public virtual string ModelName { get; protected set; }
    }
}