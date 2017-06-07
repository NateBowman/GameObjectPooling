using UnityEngine;

namespace Pooling {
    /// <summary>
    /// Extension class for access to pooling methods directly from a GameObject
    /// </summary>
    public static class PrefabFactoryPoolExtensions {
        /// <summary>
        /// Gets an instance of this gameobject from the prefab factory
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GameObject GetFromPool(this GameObject obj) {
            return PrefabFactoryPool.GetFromPool(obj);
        }

        /// <summary>
        /// Returns this prefab instance to the prefab factory pool
        /// </summary>
        /// <param name="obj"></param>
        public static void ReturnPooled(this GameObject obj) {
            PrefabFactoryPool.ReturnToPool(obj);
        }

        /// <summary>
        /// returns the IPoolable component from the gameobject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IPoolable GetIPoolableComponent(this GameObject obj) {
            return obj.GetComponent<IPoolable>();
        }
    }
}