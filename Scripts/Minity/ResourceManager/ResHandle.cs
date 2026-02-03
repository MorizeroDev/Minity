using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minity.ResourceManager.Handlers;
using Minity.ResourceManager.UsageDetector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Minity.ResourceManager
{
    public class ResHandle
    {
        public IResHandler Handler;
        public Uri Uri;
        public string UriStr;
        public IUsageDetector UsageDetector;
        public Task<Object>? LoadTask;
        public Object? Resource;

        public T GetResource<T>() where T : Object
        {
#if UNITY_EDITOR
            var ret = Resource as T;
            if (ret == null)
            {
                Debug.LogWarning($"Resource is invalid: {UriStr}");
            }
            return (Resource as T)!;
#else
            return (Resource as T)!;
#endif
        }
    }
}
