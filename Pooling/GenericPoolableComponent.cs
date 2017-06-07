using UnityEngine;

//using AdvancedInspector;

namespace Pooling {
    /// <summary>
    /// Generic Base Poolable Component.
    /// Add to a gameobject.
    /// </summary>
    public class GenericPoolableComponent : MonoBehaviour, IPoolable {
        protected int poolId = 0;

#if UNITY_EDITOR
        //[Inspect] //Advanced Inspector Attribute
        public int ObjectPoolId {
            get { return poolId; }
        }
#endif

        //[Inspect] //Advanced Inspector Attribute
        public int PrefabId {
            get { return poolId == 0 ? gameObject.GetInstanceID() : poolId; }
            set { poolId = value; }
        }

        /// <summary>
        /// Called when a GO is recycled.
        /// Use to reset properties to default state.
        /// </summary>
        public virtual void Reset() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// To be called once a GO has been returned from the pool.
        /// </summary>
        public virtual void Restart() {
            gameObject.SetActive(true);
        }

        public virtual void Update() {}
    }
}