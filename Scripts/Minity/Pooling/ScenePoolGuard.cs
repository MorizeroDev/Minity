using System;
using System.Collections.Generic;
using System.Text;
using Minity.Logger;
using Minity.General;
using UnityEngine;

namespace Minity.Pooling
{
    [AddComponentMenu("")]
    internal class ScenePoolGuard : MonoBehaviour
    {
        public static ScenePoolGuard Instance { get; private set; }
        
        internal static readonly List<EnumIdentifier> PrefabInScene = new List<EnumIdentifier>();
        internal readonly StringBuilder DestroyRecords = new StringBuilder();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            foreach (var prefab in PrefabInScene)
            {
                ObjectPool.contexts[prefab].Clear();
            }
        }

        private void Update()
        {
            if (DestroyRecords.Length > 0)
            {
                DebugLog.LogError("Several poolable objects were unexpectedly destroyed, this will break the object pool!\n" +
                               DestroyRecords);
                DestroyRecords.Clear();
            }
        }
    }
}
