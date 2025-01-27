using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minity.Variable
{
    public class MinityVariableGuard : MonoBehaviour
    {
        internal static MinityVariableGuard Instance;
        internal static readonly Stack<MinityVariableBase> ChangedSinceLastUpdate = new(); 
        internal static readonly Stack<MinityVariableBase> ChangedSinceLastFixedUpdate = new(); 
        
        [RuntimeInitializeOnLoadMethod]
        internal static void EnsureInitialized()
        {
            if (Instance)
            {
                return;
            }

            var go = new GameObject("[Minity Variable Guard]", typeof(MinityVariableGuard));
            go.SetActive(true);
            Instance = go.GetComponent<MinityVariableGuard>();
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
