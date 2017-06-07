using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Pooling {
    /// <summary>
    /// Static GameObject pooling class
    /// </summary>
    public static class PrefabFactoryPool {
        /// <summary>
        /// Empty GameObject container for "Recycled" GameObjects.
        /// </summary>
        private static GameObject gameObjectContainer;

        /// <summary>
        /// Collection of all GameObjectPool's.
        /// </summary>
        private static Dictionary<int, GameObjectPool> PrefabPool = new Dictionary<int, GameObjectPool>();

        /// <summary>
        /// Returns an instance of a Pooled Prefab.
        /// </summary>
        /// <param name="prefab">Prefab to return an instance of.</param>
        /// <returns></returns>
        public static GameObject GetFromPool(GameObject prefab) {
            //Get the IPoolable component from the Prefab or Add a GenericPoolableComponent if none is attached.
            var poolableComponent = prefab.GetIPoolableComponent() ?? prefab.AddComponent<GenericPoolableComponent>();
            Assert.IsNotNull(poolableComponent);

            //Check if a scene GameObject container and GameObjectPool for the prefab exist. Create them if they do not.
            CheckOrSetupValidPool(poolableComponent, prefab);

            //Return a pooled or new instance of the prefab.
            return PrefabPool[poolableComponent.PrefabId].RemoveOrDefault();
        }

        /// <summary>
        /// Returns a GameObject to its GameObjectPool.
        /// </summary>
        /// <param name="go">GameObject to return to its Pool.</param>
        public static void ReturnToPool(GameObject go) {
            //Get the IPoolable component from the Prefab or Add a GenericPoolableComponent if none is attached.
            var poolableComponent = go.GetIPoolableComponent() ?? go.AddComponent<GenericPoolableComponent>();
            Assert.IsNotNull(poolableComponent);

            //Check if a scene GameObject containers and GameObjectPool for the prefab exist. Create them if they do not.
            CheckOrSetupValidPool(poolableComponent, go);

            //Return GameObject to its pool.
            PrefabPool[poolableComponent.PrefabId].Recycle(go);
        }

        /// <summary>
        /// Creates a Pool for <paramref name="gameObject"/> if none exists.
        /// Creates the GameObject container for pooled GameObjects.
        /// </summary>
        /// <param name="poolableComponent"></param>
        /// <param name="gameObject"></param>
        private static void CheckOrSetupValidPool(IPoolable poolableComponent, GameObject gameObject) {
            //Check for and create a GameObject to parent/contain "recycled" GameObject's.
            if (gameObjectContainer == null) gameObjectContainer = new GameObject("Pool Container");

            //Check for an existing pool for gameObject. Add a new pool if one does not exist.
            if (!PrefabPool.ContainsKey(poolableComponent.PrefabId))
                PrefabPool.Add(poolableComponent.PrefabId, new GameObjectPool(gameObject));
        }


        /// <summary>
        /// Pool of a single type (InstanceID) of GameObject
        /// </summary>
        private class GameObjectPool {
            /// <summary>
            /// Queue of all currently pooled instances.
            /// </summary>
            private readonly Queue<GameObject> gameObjects;

            /// <summary>
            /// Empty GameObject container for "Recycled" GameObjects of this InstanceID.
            /// </summary>
            private readonly GameObject parentGameObject;

            /// <summary>
            /// Ref to the Prefab that this pool holds.
            /// </summary>
            private readonly GameObject prefab;

            /// <summary>
            /// InstanceID of the prefab.
            /// </summary>
            private readonly int prefabId;

            /// <summary>
            /// Creates a new pool based on <paramref name="gameObjectPrefab"/>.
            /// </summary>
            /// <param name="gameObjectPrefab">Prefab to pool</param>
            public GameObjectPool(GameObject gameObjectPrefab) {
                gameObjects = new Queue<GameObject>();
                prefab = gameObjectPrefab;
                prefabId = prefab.GetInstanceID();

                parentGameObject = new GameObject(prefab.name);
                parentGameObject.transform.parent = gameObjectContainer.transform;
            }

            /// <summary>
            /// Count of currently pooled objects.
            /// </summary>
            public int CurrentUse {
                get { return gameObjects.Count; }
            }

            /// <summary>
            /// Returns true if the pool is empty.
            /// </summary>
            public bool IsEmpty {
                get { return gameObjects.Count == 0; }
            }

            /// <summary>
            /// Reset and return a GameObject to the pool.
            /// </summary>
            /// <param name="gameObject">GameObject to be recycled.</param>
            public void Recycle(GameObject gameObject) {
                var poolable = gameObject.GetIPoolableComponent();
                if (poolable != null) {
                    poolable.Reset();
                }
                else {
                    return;
                }
                gameObject.transform.parent = parentGameObject.transform;
                gameObjects.Enqueue(gameObject);
            }

            /// <summary>
            /// Return a Pooled GameObject. Returns a new instance if pool is empty
            /// </summary>
            /// <returns>GameObject</returns>
            public GameObject RemoveOrDefault() {
                if (IsEmpty) {
                    Assert.IsNotNull(prefab);
                    var go = Object.Instantiate(prefab);
                    go.GetComponent<IPoolable>().PrefabId = prefabId;
                    return go;
                }
                return gameObjects.Dequeue();
            }
        }
    }
}