using System;
using System.Collections.Generic;
using Paraparty.UnityPolyfill;
using UnityEngine;

namespace Minity.Variable
{
    public class MinityVariableGuard : MonoBehaviour
    {
        internal static MinityVariableGuard Instance;
        internal static readonly Stack<MinityVariableBase> ChangedSinceLastUpdate = new Stack<MinityVariableBase>(); 
        internal static readonly Stack<MinityVariableBase> ChangedSinceLastFixedUpdate = new Stack<MinityVariableBase>(); 
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void EnsureInitialized()
        {
            if (Instance)
            {
                return;
            }

            var go = new GameObject("[Minity Variable Guard]", typeof(MinityVariableGuard));
            go.SetActive(true);
            Instance = go.GetComponent<MinityVariableGuard>();
            DontDestroyOnLoad(go);
        }

        private void FixedUpdate()
        {
            while (ChangedSinceLastFixedUpdate.TryPop(out var variable))
            {
                variable.ChangedSinceLastFixedUpdate = false;
            }
        }

        private void LateUpdate()
        {
            while (ChangedSinceLastUpdate.TryPop(out var variable))
            {
                variable.ChangedSinceLastUpdate = false;
            }
        }
    }
}
