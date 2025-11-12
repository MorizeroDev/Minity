using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Minity.Event
{
    public sealed class EventBus
    {
        private static readonly Lazy<EventBus> _lazyInstance = new(() => new EventBus());

        public static EventBus Instance => _lazyInstance.Value;

        private EventBus() { }
        private readonly Dictionary<Type, IEventContainer> eventMap = new();

#if UNITY_EDITOR
        public IEnumerable<Type> GetRegisteredTypes() => eventMap.Keys;
#endif


        /// <summary>
        /// Register an event type with an internally-managed event instance.
        /// Returns the SerializableEvent&lt;T&gt; for immediate use.
        /// </summary>
        private EventCollection<T> Register<T>() where T : IEventArgs
        {
            var key = typeof(T);
            if (!eventMap.ContainsKey(key))
            {
                var evt = new EventCollection<T>();
                eventMap[key] = new EventContainer<T>(evt);
                return evt;
            }
            else
            {
                Debug.LogError($"EventBus: Event {typeof(T).Name} already registered");
                // Return existing event if already registered
                if (eventMap.TryGetValue(key, out var container) && container is EventContainer<T> typed)
                {
                    return typed.EventCollection;
                }
                return null;
            }
        }

        /// <summary>
        /// Subscribe to an event by adding a listener. Lazily registers the event type if needed.
        /// </summary>
        public void Subscribe<T>(Action<T> listener) where T : IEventArgs
        {
            var key = typeof(T);
            
            if (!eventMap.TryGetValue(key, out var container))
            {
                // Lazily register if not exists
                var evt = Register<T>();
                if (evt != null)
                {
                    evt.AddListener(listener);
                }
                else
                {
                    Debug.LogError($"EventBus: failed to register event {typeof(T).Name}");
                }
            }
            else if (container is EventContainer<T> typed)
            {
                typed.EventCollection.AddListener(listener);
            }
        }
        
        /// <summary>
        /// Unsubscribe from an event by removing a listener.
        /// If no subscribers remain, automatically unregisters the event.
        /// </summary>
        public void UnSubscribe<T>(Action<T> listener) where T : IEventArgs
        {
            var key = typeof(T);
            if (eventMap.TryGetValue(key, out var container) && container is EventContainer<T> typed)
            {
                typed.EventCollection.RemoveListener(listener);
                
                // Check if there are any remaining subscribers
                if (typed.EventCollection.GetInvocationList().Length == 0)
                {
                    eventMap.Remove(key);
                    Debug.Log($"EventBus: Event {typeof(T).Name} auto-unregistered (no subscribers remain)");
                }
            }
        }

        public void Publish<T>(T args) where T : IEventArgs
        {
            var key = typeof(T);
            if (eventMap.TryGetValue(key, out var container))
            {
                container?.Invoke(args);
            }
            else
            {
                Debug.LogWarning($"EventBus: Event {typeof(T).Name} not registered");
            }
        }


        /// <summary>
        /// Non-generic version to get subscribers by Type at runtime (useful for Editor tooling).
        /// </summary>
        public IEnumerable<SubscriberInfo> GetSubscribers(Type eventArgType)
        {
            if (eventArgType == null) return Array.Empty<SubscriberInfo>();
            if (eventMap.TryGetValue(eventArgType, out var container))
            {
                return container.GetSubscribers();
            }
            return Array.Empty<SubscriberInfo>();
        }
    }
}
