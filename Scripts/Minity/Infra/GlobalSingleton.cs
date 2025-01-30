using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minity.Infra
{
    public abstract class GlobalSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    var go = new GameObject($"[{typeof(T).Name}]", typeof(T));
                    go.SetActive(true);
                    DontDestroyOnLoad(go);
                    _instance = go.GetComponent<T>();
                }

                return _instance;
            }
        }

    }
}
