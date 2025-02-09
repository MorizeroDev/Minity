#if MINITY_ENABLE_XLUA_TOOLS
using System;
using System.Runtime.CompilerServices;
using Minity.XLuaTools.EventSupports;
using UnityEngine;
using XLua;

namespace Minity.XLuaTools.EventSupports
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(LuaBehaviour))]
    public class BasicEventSupport : MonoBehaviour, ILuaEventSupport
    {
        private Action awake, start, onDestroy;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetEvents(LuaTable table)
        {
            awake = table.Get<Action>("awake");
            start = table.Get<Action>("start");
            onDestroy = table.Get<Action>("onDestroy");
        }

        private void CleanEvents()
        {
            awake = start = onDestroy = null;
        }
        
        void ILuaEventSupport.Initialize(LuaTable table)
        {
            GetEvents(table);
            awake?.Invoke();
        }
        
        void ILuaEventSupport.Reload(LuaTable table)
        {
            GetEvents(table);
        }

        private void Start()
        {
            start?.Invoke();
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();

            CleanEvents();
        }
    }
}
#endif