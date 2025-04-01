using System;
using System.Collections.Generic;
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
            while (ChangedSinceLastFixedUpdate.Count > 0)
            {
                ChangedSinceLastFixedUpdate.Pop().ChangedSinceLastFixedUpdate = false;
            }
        }

        private void LateUpdate()
        {
            while (ChangedSinceLastUpdate.Count > 0)
            {
                ChangedSinceLastUpdate.Pop().ChangedSinceLastUpdate = false;
            }
        }
    }
}
