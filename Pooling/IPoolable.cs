namespace Pooling {
    /// <summary>
    /// Interface for all poolable objects
    /// </summary>
    public interface IPoolable {
        int PrefabId { get; set; }
        void Reset();
        void Restart();
    }
}