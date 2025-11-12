using System;
using System.Collections.Generic;

namespace Minity.Event
{
    /// <summary>
    /// Lightweight, delegate-based replacement for UnityEvent<T> used by EventBus.
    /// This avoids relying on UnityEvent while keeping a compatible API surface
    /// (AddListener / RemoveListener / Invoke).
    /// </summary>
    public class EventCollection<T> where T : IEventArgs
    {
        // internal delegate storage (use delegate field so we can inspect invocation list)
        private Action<T> listeners;

        public void AddListener(Action<T> listener) => listeners += listener;

        public void RemoveListener(Action<T> listener) => listeners -= listener;
        
        public void Invoke(T args) => listeners?.Invoke(args);

        public void RemoveAllListeners() => listeners = null;

        public Delegate[] GetInvocationList() => listeners?.GetInvocationList() ?? Array.Empty<Delegate>();

        public List<SubscriberInfo> GetSubscribers()
        {
            var list = new List<SubscriberInfo>();
            var inv = GetInvocationList();
            foreach (var d in inv)
            {
                list.Add(new SubscriberInfo(d.Target, d.Method.Name, d.Method.DeclaringType?.FullName));
            }
            return list;
        }
    }
}
