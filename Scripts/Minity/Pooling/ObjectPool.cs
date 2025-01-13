using System;
using System.Collections.Generic;
using System.Linq;
using Minity.Logger;
using Minity.Milutools.General;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minity.Pooling
{
    public static class ObjectPool
    {
        /// <summary>
        /// When enabled, the object pool will track the usage of objects and periodically release
        /// the excess, infrequently used objects generated during peak usage.
        /// However, if the number of prefabs in the pool is large,
        /// enabling this option may cause stuttering.
        /// </summary>
        public static bool AutoReleaseUnusedObjects { get; set; } = true;
        
        internal static readonly Dictionary<EnumIdentifier, PoolContext> contexts = new();
        internal static readonly Dictionary<GameObject, PoolableObject> objectDict = new();

        private static bool initialized = false;

        internal static Transform scenePoolParent { get; private set; }
        internal static Transform poolParent { get; private set; }

        private static void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            var go = new GameObject("[Object Pool]", typeof(PoolGuard));
            GameObject.DontDestroyOnLoad(go);
            go.SetActive(true);
            poolParent = go.transform;
        }

        internal static void CreateScenePoolGuard()
        {
            if (scenePoolParent)
            {
                return;
            }
            var guard = new GameObject("[Scene Object Pool]", typeof(ScenePoolGuard));
            guard.SetActive(true);
            scenePoolParent = guard.transform;
        }

        /// <summary>
        /// Return the object back to the pool
        /// </summary>
        /// <param name="gameObject"></param>
        public static void ReturnToPool(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (!objectDict.ContainsKey(gameObject))
            {
                DebugLog.LogWarning("The specific game object is not managed by the object pool.");
                GameObject.Destroy(gameObject);
                return;
            }
#endif
            objectDict[gameObject].ReturnToPool();
        }

        /// <summary>
        /// To unregister a prefab.
        /// NOTE: this will dispose all the objects in the pool of the previous registered prefab.
        /// </summary>
        /// <param name="id">an enum value to identify a specific prefab</param>
        public static void UnregisterPrefabAndDestroy<T>(T id) where T : Enum
        {
            EnsureInitialized();

            var key = EnumIdentifier.Wrap(id);
            if (contexts.TryGetValue(key, out var existing))
            {
                foreach (var obj in existing.Objects)
                {
                    obj.PoolableController.ReadyToDestroy = true;
                    UnityEngine.Object.Destroy(obj.GameObject);
                }
                contexts.Remove(key);
                
                if (existing.LifeCyclePolicy == PoolLifeCyclePolicy.DestroyOnLoad && ScenePoolGuard.Instance)
                {
                    ScenePoolGuard.PrefabInScene.Remove(key);
                }
            }
            else
            {
                Debug.LogWarning($"Prefab {key} is not registered, no need to unregister.");
            }
        }
        
        /// <summary>
        /// To ensure the prefab is registered.
        /// You must first register it before requesting a poolable object from the prefab.
        /// </summary>
        /// <param name="id">an enum value to identify a specific prefab</param>
        /// <param name="prefab">the prefab object</param>
        /// <param name="minimumObjectCount">set the minimum object count and prepare specific amount of objects beforehand</param>
        /// <param name="lifeCyclePolicy">when the prefab and its objects get destroyed</param>
        public static void EnsurePrefabRegistered<T>(T id, GameObject prefab, 
            uint minimumObjectCount,
            PoolLifeCyclePolicy lifeCyclePolicy = PoolLifeCyclePolicy.DestroyOnLoad) where T : Enum
        {
            EnsureInitialized();

            var key = EnumIdentifier.Wrap(id);
            
            // 强制检查
            if (contexts.TryGetValue(key, out var existing))
            {
                if (existing.Prefab == prefab && existing.LifeCyclePolicy == lifeCyclePolicy)
                {
                    DebugLog.LogWarning($"Prefab '{key}' is already registered.");
                    return;
                }
                
                throw new ArgumentException($"Prefab '{key}' is already registered. " +
                                            $"Each prefab must have a unique name.", nameof(id));
            }

            if (lifeCyclePolicy == PoolLifeCyclePolicy.DestroyOnLoad)
            {
                if (!ScenePoolGuard.Instance)
                {
                    CreateScenePoolGuard();
                }
                ScenePoolGuard.PrefabInScene.Add(key);
            }

            var poolableObject = prefab.GetComponent<PoolableObject>();
            if (!poolableObject)
            {
                throw new InvalidOperationException($"Prefab '{key}' must have a PoolableObject component. " +
                                                    $"Please add the component manually before registering.");
            }

            poolableObject.IsPrefab = true;
            
            var context = new PoolContext()
            {
                Prefab = prefab,
                Name = $"{typeof(T).FullName}.{id}",
                ID = id,
                LifeCyclePolicy = lifeCyclePolicy,
                MinimumObjectCount = minimumObjectCount,
                ComponentTypes = poolableObject.Components?.Where(x => x)
                                                .Select(x => x.GetType()).ToArray() ?? Array.Empty<Type>()
            };
            
            contexts.Add(key, context);
            
            //context.Prepare(minimumObjectCount);
        }

        /// <summary>
        /// Retrieve an object with the specified prefab ID from the pool and obtain its object set,
        /// including all associated components and related information.
        /// </summary>
        /// <param name="prefab">an enum value to identify a specific prefab</param>
        /// <param name="handler">an function to do something with the collection</param>
        /// <param name="parent">the parent of the retrieved object to be set</param>
        /// <returns></returns>
        public static void Request<T>(T prefab, Action<PooledEntity> handler, Transform parent = null) where T : Enum
        {
            var key = EnumIdentifier.Wrap(prefab);
            var collection = contexts[key].Request();
            collection.Transform.SetParent(parent, false);
            handler(collection);
        }
        
        /// <summary>
        /// Retrieve an object with the specified prefab ID from the pool and obtain its object set,
        /// including all associated components and related information.
        /// </summary>
        /// <param name="prefab">an enum value to identify a specific prefab</param>
        /// <param name="parent">the parent of the retrieved object to be set</param>
        /// <returns></returns>
        public static PooledEntity Request<T>(T prefab, Transform parent = null) where T : Enum
        {
            var key = EnumIdentifier.Wrap(prefab);
            var collection = contexts[key].Request();
            collection.Transform.SetParent(parent, false);
            return collection;
        }
        
        /// <summary>
        /// Retrieve an object with the specified prefab ID from the pool and obtain its GameObject.
        /// </summary>
        /// <param name="prefab">an enum value to identify a specific prefab</param>
        /// <param name="parent">the parent of the retrieved object to be set</param>
        /// <returns></returns>
        public static GameObject RequestGameObject<T>(T prefab, Transform parent = null) where T : Enum
        {
            return Request(prefab, parent).GameObject;
        }

        /// <summary>
        /// Retrieve an object with the specified prefab ID from the pool and obtain its associated primary component.
        /// </summary>
        /// <param name="prefab">an enum value to identify a specific prefab</param>
        /// <param name="parent">the parent of the retrieved object to be set</param>
        /// <typeparam name="T">Component Type</typeparam>
        /// <typeparam name="E">Prefab ID Enum</typeparam>
        /// <returns></returns>
        public static T RequestMainComponent<T, E>(E prefab, Transform parent = null) where T : Component where E : Enum
        {
            return (T)Request(prefab, parent).MainComponent;
        }
        
        /// <summary>
        /// Retrieve an object with the specified prefab ID from the pool and obtain its associated component of a specific type.
        /// </summary>
        /// <param name="prefab">an enum value to identify a specific prefab</param>
        /// <param name="parent">the parent of the retrieved object to be set</param>
        /// <typeparam name="T">Component Type</typeparam>
        /// <typeparam name="E">Prefab ID Enum</typeparam>
        /// <returns></returns>
        public static T RequestComponent<T, E>(E prefab, Transform parent = null) where T : Component where E : Enum
        {
            return Request(prefab, parent).GetComponent<T>();
        }

        /// <summary>
        /// Immediately return all objects with the specified prefab ID to the pool.
        /// </summary>
        /// <param name="prefab">an enum value to identify a specific prefab</param>
        public static void ReturnAllObjects<T>(T prefab) where T : Enum
        {
            var key = EnumIdentifier.Wrap(prefab);
            contexts[key].RecycleAllObjects();
        }
    }
}
