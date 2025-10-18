using System;
using System.Collections.Generic;
#if !NET_STANDARD_2_1
using Paraparty.UnityPolyfill;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Minity.Infra
{
    public class SceneSingleton : MonoBehaviour
    {
        private static readonly Dictionary<Type, Object> instances = new Dictionary<Type, Object>();

        public static T Get<T>() where T : Object
        {
            if (!instances.TryGetValue(typeof(T), out var instance))
            {
                throw new Exception($"'{typeof(T).Name}' instance is not registered.");
            }

            if (!instance)
            {
                throw new Exception($"'{typeof(T).Name}' instance is destroyed.");
            }

            return (T)instance;
        }

		public static bool Exist<T>() where T : Object
		{
            if (!instances.TryGetValue(typeof(T), out var instance) || !instance)
            {
                return false;
            }
			return true;
		}

        public static void Register<T>(T instance) where T : Object
        {
            var type = typeof(T);
            if (!instances.TryAdd(type, instance))
            {
                if (instances[type])
                {
                    Debug.LogWarning($"'{type.Name}' instance is duplicated.");
                }
                instances[type] = instance;
            }
        }
    }
}
