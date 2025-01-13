using System;
using System.Collections.Generic;
using System.Linq;
using Milutools.Logger;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Milutools.Pooling
{
    [DisallowMultipleComponent]
    public sealed class PoolableObject : MonoBehaviour
    {
        [FormerlySerializedAs("AutoRecycleTimeOut")]
        [FormerlySerializedAs("RecycleTimeOut")]
        [Tooltip("Automatically return the object to the pool after specific seconds.\n* -1 means disabling auto recycling.")]
        public float AutoReturnTimeOut = -1f;
        
        [FormerlySerializedAs("LinkComponent")] 
        [Tooltip("Link a main component, then you can access it using ObjectPool.Request<T, E> function.\n" +
                 "This would avoid using GetComponent function frequently as GetComponent function may made the program slow.")]
        public Component MainComponent;
        
        [FormerlySerializedAs("DictComponent")] 
        [Tooltip("Link more components, then you can access it using PoolableEntity.GetComponent function.\n" +
                 "This would avoid using GetComponent function frequently as GetComponent function may made the program slow.\n" +
                 "NOTICE: involving multiple components with a same type is not supported.")]
        public Component[] Components;

        public UnityEvent OnReset;
        
        internal bool Using { get; set; }
        internal bool IsPrefab { get; set; } = false;
        internal bool ReadyToDestroy = false;
        
        private PooledEntity _pooledEntity;
        private PoolContext _parentContext;
        private int _objectHash;
        
        private float returnTick = 0f;
        
#if UNITY_EDITOR
        private bool warned = false;
#endif
        
        internal void Initialize(PoolContext context, PooledEntity collection)
        {
            _parentContext = context;
            _pooledEntity = collection;
            _objectHash = GetHashCode();
            
            DebugLog.Log($"PoolableObject created: Hash={_objectHash}, Name={gameObject.name}, PrefabName={context.Name}");
        }
        
        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (IsPrefab)
            {
                throw new Exception(
                    "The prefab object is unexpectedly destroyed, the object pool would fail when producing new objects!");
            }
            
            if (_parentContext == null)
            {
                return;
            }

            if (!ReadyToDestroy)
            {
                if (_parentContext.LifeCyclePolicy == PoolLifeCyclePolicy.Eternity)
                {
                    throw new Exception($"PoolableObject is unexpectedly destroyed, this has broken the object pool: Hash={_objectHash}, Name={gameObject.name}, PrefabName={_parentContext.Name}");
                }
                else
                {
                    ScenePoolGuard.Instance.DestroyRecords.AppendLine($"Hash={_objectHash}, Name={gameObject.name}, PrefabName={_parentContext.Name}");
                }
            }
            else
            {
                if (Using)
                {
                    _parentContext.CurrentUsage--;
                }
            }

            ObjectPool.objectDict.Remove(gameObject);
            
            DebugLog.Log($"PoolableObject destroyed: Hash={_objectHash}, Name={gameObject.name}, PrefabName={_parentContext.Name}");
#endif
        }

        /// <summary>
        /// Use ReturnToPool() instead.
        /// </summary>
        [Obsolete]
        public void WaitForRecycle()
            => ReturnToPool();

        /// <summary>
        /// Return the object to the pool
        /// </summary>
        public void ReturnToPool()
        {
            if (!Using)
            {
#if UNITY_EDITOR
                DebugLog.LogWarning("The object is not using, returning back to pool is not necessary.");
#endif
                return;
            }
#if UNITY_EDITOR
            if (IsPrefab)
            {
                DebugLog.LogError("You are trying to return a prefab to the pool, this is not allowed.");
                return;
            }
            if (_parentContext == null)
            {
                Destroy(gameObject);
                DebugLog.LogWarning("You are trying to return an object that is not managed by the object pool, this is not allowed.");
                return;
            }
#endif
            Using = false;
            _pooledEntity.Transform.SetParent(null);
            _parentContext.ReturnToPool(_pooledEntity);
        }
        
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (IsPrefab)
            {
                DebugLog.LogWarning("The prefab for the object pool must be inactive.");
                return;
            }
            
            if (_parentContext == null && !warned)
            {
                warned = true;
                DebugLog.LogWarning("This object is not managed by the object pool, please use 'ObjectPool.Request' function to create the object.");
                return;
            }
#endif
            
            if (AutoReturnTimeOut <= 0f)
            {
                return;
            }
            
            returnTick += Time.fixedDeltaTime;
            if (returnTick >= AutoReturnTimeOut)
            {
                ReturnToPool();
                returnTick = 0f;
            }
        }
    }
}
