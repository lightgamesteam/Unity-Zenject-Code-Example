namespace Module.Core.Patterns {
    public abstract class Singleton<T> where T : new() {
        public static T Instance { get; } = new T();
    }
}