using UnityEngine.Events;
using System;

namespace Minity.Event
{
    [Serializable]
    public class SerializableEvent<T> : UnityEvent<T> where T : IEventArgs { }
}
