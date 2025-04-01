using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minity.Pooling
{
    public enum PoolLifeCyclePolicy
    {
        /// <summary>
        /// Until the process exits
        /// </summary>
        Eternity,
        /// <summary>
        /// Destroy collectively upon scene transition
        /// </summary>
        DestroyOnLoad
    }

    public class PooledEntity
    {
        public GameObject GameObject { get; internal set; }
        public Transform Transform { get; internal set; }
        public PoolableObject PoolableController { get; internal set; }
        internal Component MainComponent { get; set; }
        internal Dictionary<Type, Component> Components { get; } = new Dictionary<Type, Component>();

        public T GetComponent<T>() where T : Component
        {
            return (T)Components[typeof(T)];
        }
        
        public T GetMainComponent<T>() where T : Component
        {
            return (T)MainComponent;
        }
    }
}
