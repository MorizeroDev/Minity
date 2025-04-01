using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Minity.UI
{
    public class BindingViewGuard : MonoBehaviour
    {
        internal static BindingViewGuard Instance;
        internal static readonly HashSet<Action> ScheduledUpdates = new HashSet<Action>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureCreated()
        {
            if (Instance)
            {
                return;
            }

            var go = new GameObject("[Binding View Guard]", typeof(BindingViewGuard));
            Instance = go.GetComponent<BindingViewGuard>();
            go.SetActive(true);
            
            DontDestroyOnLoad(go);
        }

        internal static void ScheduleUpdate(Action updateAction)
        {
            EnsureCreated();
            ScheduledUpdates.Add(updateAction);
        }

        private void LateUpdate()
        {
            foreach (var action in ScheduledUpdates)
            {
                action.Invoke();
            }
            
            ScheduledUpdates.Clear();
        }
    }
}
