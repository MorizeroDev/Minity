using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Minity.Event
{
    /// <summary>
    /// Internal non-generic container interface for storing events inside EventBus.
    /// Using this avoids keeping raw object values in the dictionary and centralizes
    /// invocation logic in a typed wrapper.
    /// </summary>
    internal interface IEventContainer
    {
        void Invoke(IEventArgs args);
        IEnumerable<SubscriberInfo> GetSubscribers();
    }

    internal class EventContainer<T> : IEventContainer where T : IEventArgs
    {
        private readonly EventCollection<T> evt;
        public EventCollection<T> EventCollection => evt;

        public EventContainer(EventCollection<T> evt)
        {
            this.evt = evt ?? throw new ArgumentNullException(nameof(evt));
        }

        public void Invoke(IEventArgs args)
        {
            if (args is T typed)
            {
                evt.Invoke(typed);
            }
        }
        
        public IEnumerable<SubscriberInfo> GetSubscribers() => evt.GetSubscribers();
    }
}
