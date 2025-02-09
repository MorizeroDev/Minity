#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using XLua;

namespace Minity.XLuaTools.EventSupports
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(LuaBehaviour))]
    public class EnablingEventSupport : MonoBehaviour, ILuaEventSupport
    {
        private Action onEnable, onDisable;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetEvents(LuaTable table)
        {
            onEnable = table.Get<Action>("onEnable");
            onDisable = table.Get<Action>("onDisable");
        }

        private void CleanEvents()
        {
            onEnable = onDisable = null;
        }
        
        void ILuaEventSupport.Initialize(LuaTable table)
        {
            GetEvents(table);
        }

        void ILuaEventSupport.Reload(LuaTable table)
        {
            GetEvents(table);
        }

        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        private void OnDisable()
        {
            onDisable?.Invoke();
        }

        private void OnDestroy()
        {
            CleanEvents();
        }
    }
}
#endif