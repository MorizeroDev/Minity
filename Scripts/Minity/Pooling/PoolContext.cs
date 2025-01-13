using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Minity.Logger;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Minity.Pooling
{
    public class PoolContext
    {
        public GameObject Prefab { get; internal set; }
        public string Name { get; internal set; }
        public IReadOnlyList<PooledEntity> AllObjects => Objects;
        public PoolLifeCyclePolicy LifeCyclePolicy { get; internal set; }
        public uint MinimumObjectCount { get; internal set; }
        
        internal List<PooledEntity> Objects { get; } = new();
        
        internal Type[] ComponentTypes;
        internal object ID;
        
        internal readonly Queue<uint> UsageRecords = new();
        internal uint PeriodUsage = 0;
        internal uint CurrentUsage = 0;
        internal uint IdleTick = 0;
        
        private Stack<PooledEntity> _objectStack { get; } = new();
        
        public T GetID<T>() where T : Enum
        {
            return (T)ID;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Transform GetPoolParent()
        {
            return LifeCyclePolicy switch
            {
                PoolLifeCyclePolicy.Eternity => ObjectPool.poolParent,
                PoolLifeCyclePolicy.DestroyOnLoad => ObjectPool.scenePoolParent,
                _ => null
            };
        }
        
        private PooledEntity Produce()
        {
            var gameObject = Object.Instantiate(Prefab, GetPoolParent());
            gameObject.name = $"[RE{gameObject.GetInstanceID()}] {Name}";
            gameObject.SetActive(false);
            
            var poolableObject = gameObject.GetComponent<PoolableObject>();
            ObjectPool.objectDict.Add(gameObject, poolableObject);
            
            if (LifeCyclePolicy == PoolLifeCyclePolicy.Eternity)
            {
                Object.DontDestroyOnLoad(gameObject);
            }

            var collection = new PooledEntity()
            {
                GameObject = gameObject,
                Transform = gameObject.transform,
                PoolableController = poolableObject,
                MainComponent = poolableObject.MainComponent
            };

            for (var i = 0; i < ComponentTypes.Length; i++)
            {
#if UNITY_EDITOR
                if (collection.Components.ContainsKey(ComponentTypes[i]))
                {
                    DebugLog.LogError($"You are trying to link multiple components with a same type '{ComponentTypes[i]}'\n" +
                                      $", this is not supported. (ID: {ID})");
                    continue;
                }
#endif
                collection.Components.Add(ComponentTypes[i], poolableObject.Components[i]);
            }

            poolableObject.Initialize(this, collection);
            
            Objects.Add(collection);
            
            return collection;
        }

        internal void RecycleAllObjects()
        {
            foreach (var obj in AllObjects)
            {
                if (!obj.PoolableController.Using)
                {
                    continue;
                }
                obj.PoolableController.ReturnToPool();
            }
        }

        internal void Prepare(uint count)
        {
            for (var i = 0; i < count; i++)
            {
                _objectStack.Push(Produce());
            }
        }

        internal void Clear()
        {
            _objectStack.Clear();
            Objects.Clear();
            CurrentUsage = 0;
        }

        internal PooledEntity Request()
        {
            if (LifeCyclePolicy == PoolLifeCyclePolicy.DestroyOnLoad)
            {
                if (!ScenePoolGuard.Instance)
                {
                    ObjectPool.CreateScenePoolGuard();
                }
            }
            
            if (!_objectStack.TryPop(out var collection))
            {
                collection = Produce();
            }
            else
            {
                collection.PoolableController.OnReset?.Invoke();
            }

            CurrentUsage++;
            collection.PoolableController.Using = true;
            return collection;
        }

        internal void ReturnToPool(PooledEntity collection)
        {
            CurrentUsage--;
            collection.Transform.SetParent(GetPoolParent());
            collection.GameObject.SetActive(false);
            _objectStack.Push(collection);
        }

        internal int GetObjectCount()
            => _objectStack.Count;
    }
}
