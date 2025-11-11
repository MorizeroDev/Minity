using System;
using System.Collections.Generic;
using UnityEngine;
using Minity.Infra;
using UnityEngine.Events;

namespace Minity.Event
{
    public class EventBus : GlobalSingleton<EventBus>
    {
        private readonly Dictionary<Type, UnityEventBase> eventMap = new();

#if UNITY_EDITOR
        private readonly HashSet<Type> registeredTypes = new();
        public IEnumerable<Type> GetRegisteredTypes() => registeredTypes;
#endif

        public void Register<T>(SerializableEvent<T> evt) where T : IEventArgs
        {
            if (!eventMap.ContainsKey(typeof(T)))
            {
                eventMap[typeof(T)] = evt;
#if UNITY_EDITOR
                registeredTypes.Add(typeof(T));
#endif
            }
            else
            {
                Debug.LogError($"EventBus: Event {typeof(T).Name} already registered");
            }
        }

        public void Unregister<T>() where T : IEventArgs
        {
            if (eventMap.ContainsKey(typeof(T)))
            {
                eventMap.Remove(typeof(T));
#if UNITY_EDITOR
                registeredTypes.Remove(typeof(T));
#endif
                Debug.Log($"Event {typeof(T).Name} removed safely!");
            }
        }

        public void Publish<T>(T args) where T : IEventArgs
        {
            if (eventMap.TryGetValue(typeof(T), out var evt))
            {
                (evt as SerializableEvent<T>)?.Invoke(args);
            }
            else
            {
                Debug.LogWarning($"EventBus: Event {typeof(T).Name} not registered");
            }
        }
    }
}
